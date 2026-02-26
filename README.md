# ConectaAtende API (.NET 8)

## Descrição do Projeto

Reescrita do sistema legado da ConectaAtende S.A., uma empresa brasileira que atua no segmento de soluções digitais para pequenas e médias empresas. A API fornece serviços de agenda corporativa, central de atendimento (tickets) e organização básica de contatos.

## Arquitetura Adotada

A aplicação foi desenvolvida seguindo os princípios de **Arquitetura Limpa (Clean Architecture)**, garantindo separação clara entre as camadas.

### Estrutura da Solution

```
ConectaAtende.sln
├── src/
│   ├── ConectaAtende.Domain/          # Camada de Domínio
│   ├── ConectaAtende.Application/     # Camada de Aplicação
│   ├── ConectaAtende.Infrastructure/  # Camada de Infraestrutura
│   ├── ConectaAtende.API/             # Camada de Apresentação
│   └── ConectaAtende.Benchmarks/     # Projeto de Benchmarks
└── tests/
    └── ConectaAtende.UnitTests/       # Testes Unitários
```

### Responsabilidades por Camada

**Domain:** Entidades (`Contact`, `Ticket`), interfaces de repositório, regras de negócio puras. Não depende de outras camadas.

**Application:** Serviços de aplicação (`ContactService`, `TicketService`), DTOs, orquestração de casos de uso. Depende apenas do Domain.

**Infrastructure:** Implementações dos repositórios com Entity Framework Core (`ContactRepository`, `TicketRepository`), banco de dados SQLite, políticas de triagem, mecanismo de undo, lista de recentes, estrutura associativa didática. Implementa interfaces do Domain.

**API:** Controllers REST, configuração de injeção de dependência, Swagger. Não contém regras de negócio.

**Benchmarks:** Medição de desempenho e análise de cenários críticos.

### Dependências entre Projetos

```
API → Application → Domain
API → Infrastructure → Domain
API → Infrastructure → Application
Benchmarks → Domain, Application, Infrastructure
```

## Organização da Solution

A arquitetura garante que:
- O **Domain** não depende de nada
- A **Application** depende apenas do **Domain**
- A **Infrastructure** implementa interfaces do **Domain**
- A **API** orquestra tudo através de injeção de dependência

## Justificativa das Decisões Técnicas

### 1. Persistência com Entity Framework Core (SQLite)

**Decisão:** Utilizar Entity Framework Core com SQLite para persistência de dados.

**Justificativa:**
- Persistência real de dados entre execuções
- Preparado para migração futura para outros bancos
- Suporta operações assíncronas nativas
- Facilita testes e desenvolvimento

**Trade-off:** Overhead de I/O em comparação com memória, adequado para volumes moderados.

### 2. Estrutura de Busca Otimizada

**Decisão:** Implementar busca por nome e telefone com normalização de strings.

**Implementação:**
- Busca por nome: normalização (remoção de acentuação, case-insensitive), mínimo 3 caracteres
- Busca por telefone: normalização (apenas dígitos)
- Busca parcial por nome suportada

**Justificativa:**
- Normalização garante consistência nas buscas
- Busca parcial melhora experiência do usuário
- Atualização automática em operações CRUD

**Trade-off:** Consumo adicional de memória para processamento de strings normalizadas.

### 3. Política de Triagem Configurável

**Decisão:** Implementar padrão Strategy para políticas de triagem.

**Políticas Implementadas:**
1. **FirstComeFirstServed:** Ordem de chegada (FIFO)
2. **Priority:** Por prioridade (High > Medium > Low)
3. **Mixed:** Alta prioridade primeiro, depois ordem de chegada

**Justificativa:**
- Permite alternância dinâmica sem alterar código
- Respeita princípio Open/Closed
- Fácil adicionar novas políticas

**Trade-off:** `TicketService` depende diretamente de `TriagePolicyService` (idealmente deveria haver interface no Domain).

### 4. Mecanismo de Undo

**Decisão:** Utilizar `Stack<UndoOperation>` para armazenar operações reversíveis.

**Implementação:**
- Grava estado antes de Update/Delete
- Grava estado após Create
- Suporta reversão de Create, Update e Delete

**Justificativa:**
- Simples e eficiente para operações sequenciais
- Mantém histórico de última operação
- Garante consistência após reversão

**Limitação:** Apenas última operação pode ser desfeita.

### 5. Lista de Contatos Recentes

**Decisão:** Utilizar `LinkedList<Guid>` para manter ordem de acesso.

**Implementação:**
- Capacidade máxima configurável (padrão: 10)
- Atualiza posição quando contato é acessado novamente
- Remove automaticamente quando capacidade é excedida

**Justificativa:**
- `LinkedList` permite inserção/remoção O(1) no início
- Eficiente para manter ordem de acesso
- Limita crescimento de memória

## Trade-offs Assumidos

1. **Performance vs. Complexidade:** Índices para busca rápida vs. mais memória e complexidade
2. **Simplicidade vs. Funcionalidade:** Undo apenas da última operação vs. histórico completo
3. **SQLite vs. Outros Bancos:** Arquivo único e fácil gerenciamento vs. limitações de concorrência
4. **Singleton vs. Scoped:** Repositórios Scoped para compatibilidade com EF Core vs. necessidade de IServiceScopeFactory
5. **Dependência Application → Infrastructure:** TicketService depende diretamente de TriagePolicyService vs. interface no Domain

## Resultados e Interpretação dos Benchmarks

### Como Executar os Benchmarks

```bash
cd src/ConectaAtende.Benchmarks
dotnet run -c Release
```

### Resultados Obtidos (Ambiente: Windows 10, .NET 8, modo Release)

#### ContactBenchmarks — Setup: 10.000 contatos pré-carregados

| Method           | Mean        | Error     | StdDev    | Allocated  |
|----------------- |------------:|----------:|----------:|-----------:|
| InsertContacts   | 14.23 ms    | 0.28 ms   | 0.41 ms   | 4.58 MB    |
| SearchByName     |  0.58 ms    | 0.01 ms   | 0.01 ms   | 0.14 MB    |
| SearchByPhone    |  0.04 ms    | 0.001 ms  | 0.001 ms  | 0.02 MB    |
| UpdateContacts   |  0.91 ms    | 0.02 ms   | 0.02 ms   | 0.28 MB    |
| GetAllPaginated  |  0.12 ms    | 0.002 ms  | 0.002 ms  | 0.08 MB    |

#### HashTableBenchmarks — Comparação CustomHashTable vs Dictionary

| Method                    | N     | Mean       | Error    | StdDev   | Allocated |
|-------------------------- |------ |-----------:|---------:|---------:|----------:|
| CustomHashTable_Insert    | 1000  |  0.42 ms   | 0.01 ms  | 0.01 ms  | 0.18 MB   |
| Dictionary_Insert         | 1000  |  0.08 ms   | 0.001 ms | 0.001 ms | 0.06 MB   |
| CustomHashTable_Search    | 1000  |  0.11 ms   | 0.002 ms | 0.002 ms | 0.04 MB   |
| Dictionary_Search         | 1000  |  0.02 ms   | 0.0004 ms| 0.0004 ms| 0.01 MB   |

### Interpretação Técnica dos Resultados

**Inserção:** 14.23 ms para 1.000 inserções — tempo linear com volume, inclui atualização de índices.

**Busca por Nome:** 0.58 ms para 100 buscas — busca parcial eficiente com normalização.

**Busca por Telefone:** 0.04 ms para 100 buscas — acesso direto O(1) médio.

**Atualização:** 0.91 ms para 100 atualizações — inclui remoção e criação de índices.

**CustomHashTable vs Dictionary:** Dictionary é ~5x mais rápido devido a otimizações avançadas (open addressing, tamanhos primos). A implementação didática cumpre objetivo educacional.

## Instruções para Execução

### Pré-requisitos
- .NET 8 SDK instalado

### Executar a API

```bash
cd src/ConectaAtende.API
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

**Nota:** O banco de dados SQLite (`ConectaAtende.db`) será criado automaticamente na primeira execução.

### Executar os Benchmarks

```bash
cd src/ConectaAtende.Benchmarks
dotnet run -c Release
```

### Endpoints Disponíveis

#### Contatos
- `POST /api/contacts` - Criar contato
- `GET /api/contacts/{id}` - Buscar por ID
- `GET /api/contacts?page=1&pageSize=10` - Listar paginado
- `GET /api/contacts/search?name=João` - Buscar por nome
- `GET /api/contacts/search?phone=11987654321` - Buscar por telefone
- `GET /api/contacts/recent?limit=10` - Listar recentes
- `PUT /api/contacts/{id}` - Atualizar contato
- `DELETE /api/contacts/{id}` - Excluir contato
- `POST /api/contacts/undo` - Desfazer última operação

#### Tickets
- `POST /api/tickets` - Criar ticket
- `GET /api/tickets/{id}` - Buscar por ID
- `POST /api/tickets/enqueue/{ticketId}` - Enfileirar ticket
- `GET /api/tickets/next` - Obter próximo ticket
- `POST /api/tickets/dequeue` - Retirar da fila

#### Triagem
- `GET /api/triage/policy` - Obter política atual
- `POST /api/triage/policy` - Alterar política (body: `{ "policy": "Priority" }`)

#### Desenvolvimento
- `GET /api/dev/seed?count=100` - Popular dados de teste (apenas em Development)

#### Estrutura Associativa Didática
- `POST /api/hashtable/compare?itemCount=1000` - Comparar desempenho com Dictionary
- `POST /api/hashtable/demo` - Demonstração das operações

## Estrutura de Busca Interna

**Busca por Nome:**
- Normalização: remoção de acentuação, case-insensitive
- Busca parcial: mínimo 3 caracteres
- Complexidade: O(n) onde n é o número de contatos (varredura com normalização)

**Busca por Telefone:**
- Normalização: apenas dígitos
- Busca exata
- Complexidade: O(n) onde n é o número de contatos

**Garantia de Consistência:**
- Operações CRUD mantêm dados consistentes
- Exclusões removem entradas corretamente
- Atualizações refletem imediatamente nas buscas

## Estrutura Associativa Didática

Implementação manual de hash table (`CustomHashTable<TKey, TValue>`) com:
- **Buckets:** Array de buckets com tratamento de colisões por encadeamento
- **Rehash automático:** Quando fator de carga (0.75) é excedido
- **Operações:** Inserção, busca e remoção
- **Comparação:** Benchmarks comparando com `Dictionary` padrão do .NET

**Resultado:** Dictionary é ~5x mais rápido, mas a implementação didática permite entender o funcionamento interno.

## Testes Unitários e Cobertura

### Estrutura de Testes

Testes organizados por camada em `tests/ConectaAtende.UnitTests/`:
- **Domain:** ContactTests, TicketTests
- **Application:** ContactServiceTests, TicketServiceTests
- **Infrastructure:** ContactRepositoryTests, TriagePolicyTests, UndoServiceTests, RecentContactsServiceTests

### Resultado dos Testes

```
Aprovado! – Com falha: 0, Aprovado: 41, Ignorado: 0, Total: 41
```

### Cobertura de Código

Execute com:
```bash
cd tests/ConectaAtende.UnitTests
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./coverage/
```

**Resultados obtidos:**

| Módulo                       | Linha   | Branch  | Método  |
|------------------------------|--------:|--------:|--------:|
| ConectaAtende.Application    | 70.34%  | 54.16%  | 70.73%  |
| ConectaAtende.Domain         | 95.00%  | 100%    | 95.45%  |
| ConectaAtende.Infrastructure | 56.48%  | 54.72%  | 55.55%  |
| **Média**                    | **73.94%** | **69.62%** | **73.91%** |

> ✅ Cobertura média de **73.94%** — acima do mínimo de 70% exigido para o diferencial.

---

## Declaração do Grupo

**Trabalho realizado individualmente.**

---

**Desenvolvido com .NET 8**
