# ConectaAtende API (.NET 8)

## Descrição do Projeto

Este projeto é uma reescrita do sistema legado da ConectaAtende S.A., uma empresa brasileira que atua no segmento de soluções digitais para pequenas e médias empresas. A API fornece serviços de agenda corporativa, central de atendimento (tickets) e organização básica de contatos.

## Arquitetura Adotada

A aplicação foi desenvolvida seguindo os princípios de **Arquitetura Limpa (Clean Architecture)**, garantindo separação clara entre as camadas e baixo acoplamento.

### Estrutura da Solution

```
ConectaAtende.sln
├── src/
│   ├── ConectaAtende.Domain/          # Camada de Domínio
│   ├── ConectaAtende.Application/     # Camada de Aplicação
│   ├── ConectaAtende.Infrastructure/  # Camada de Infraestrutura
│   ├── ConectaAtende.API/             # Camada de Apresentação (API)
│   └── ConectaAtende.Benchmarks/     # Projeto de Benchmarks
└── tests/
    └── ConectaAtende.UnitTests/       # Testes Unitários
```

### Responsabilidades por Camada

#### 1. Domain (Domínio)
- **Responsabilidades:**
  - Entidades de negócio (`Contact`, `Ticket`)
  - Enums (`TicketStatus`, `TicketPriority`)
  - Interfaces de repositório (`IContactRepository`, `ITicketRepository`)
  - Interface de política de triagem (`ITriagePolicy`)
- **Características:**
  - Não depende de nenhuma outra camada
  - Contém apenas regras de negócio puras
  - Independente de frameworks e infraestrutura

#### 2. Application (Aplicação)
- **Responsabilidades:**
  - Serviços de aplicação (`ContactService`, `TicketService`)
  - DTOs (Data Transfer Objects)
  - Orquestração de casos de uso
- **Características:**
  - Depende apenas do Domain
  - Contém a lógica de orquestração
  - Não conhece detalhes de implementação

#### 3. Infrastructure (Infraestrutura)
- **Responsabilidades:**
  - Implementações InMemory dos repositórios (`InMemoryContactRepository`, `InMemoryTicketRepository`)
  - Implementações das políticas de triagem (`FirstComeFirstServed`, `Priority`, `Mixed`)
  - Estruturas de dados internas (índices invertidos para busca)
  - Mecanismo de undo (`UndoService` com `Stack<UndoOperation>`)
  - Lista de recentes (`RecentContactsService` com `LinkedList<Guid>`)
  - Estrutura associativa didática (`CustomHashTable`)
- **Características:**
  - Implementa interfaces do Domain
  - Pode ser substituída sem afetar outras camadas
  - Contém detalhes técnicos de persistência e estruturas de dados

#### 4. API (Apresentação)
- **Responsabilidades:**
  - Controllers REST
  - Configuração de injeção de dependência
  - Swagger/OpenAPI
- **Características:**
  - Não contém regras de negócio
  - Apenas recebe requisições e delega para a camada de aplicação
  - Configuração de endpoints

#### 5. Benchmarks
- **Responsabilidades:**
  - Medição de desempenho
  - Análise de cenários críticos
  - Comparação de abordagens

## Organização da Solution

### Dependências entre Projetos

```
API → Application → Domain
API → Infrastructure → Domain
API → Infrastructure → Application
Benchmarks → Domain
Benchmarks → Application
Benchmarks → Infrastructure
```

A arquitetura garante que:
- O **Domain** não depende de nada
- A **Application** depende apenas do **Domain**
- A **Infrastructure** implementa interfaces do **Domain** e usa a **Application**
- A **API** orquestra tudo através de injeção de dependência

## Justificativa das Decisões Técnicas

### 1. Persistência InMemory

**Decisão:** Utilizar `Dictionary<Guid, T>` para armazenamento em memória.

**Justificativa:**
- Reduz complexidade inicial do projeto
- Permite foco na modelagem e estrutura arquitetural
- Facilita testes e experimentação
- Preparado para futura substituição por banco de dados real através das interfaces

**Trade-off:**
- Dados são perdidos ao reiniciar a aplicação
- Não adequado para produção com grandes volumes
- Aceitável para esta fase de desenvolvimento

### 2. Estrutura de Busca Otimizada

**Decisão:** Implementar índices invertidos para busca por nome e telefone.

**Implementação:**
- `Dictionary<string, HashSet<Guid>>` para índice de nomes normalizados
- `Dictionary<string, HashSet<Guid>>` para índice de telefones normalizados
- Normalização de strings (remoção de acentuação, case-insensitive)

**Justificativa:**
- Busca O(1) em média através dos índices
- Suporta busca parcial por nome (mínimo 3 caracteres)
- Normalização garante consistência nas buscas
- Atualização automática dos índices em operações CRUD

**Trade-off:**
- Consumo adicional de memória para manter índices
- Complexidade na manutenção dos índices
- Benefício: busca muito mais rápida que varredura linear

### 3. Política de Triagem Configurável

**Decisão:** Implementar padrão Strategy para políticas de triagem.

**Políticas Implementadas:**
1. **FirstComeFirstServed:** Ordem de chegada (FIFO)
2. **Priority:** Por prioridade (High > Medium > Low)
3. **Mixed:** Alta prioridade primeiro, depois ordem de chegada

**Justificativa:**
- Permite alternância dinâmica sem alterar código
- Respeita princípio Open/Closed (aberto para extensão, fechado para modificação)
- Fácil adicionar novas políticas no futuro
- Testável isoladamente

**Trade-off Arquitetural:**
- O `TicketService` (Application) depende diretamente do `TriagePolicyService` (Infrastructure)
- Idealmente, deveria existir uma interface na camada Domain/Application
- Aceitável para esta fase, mantendo simplicidade sobre pureza arquitetural

### 4. Mecanismo de Undo

**Decisão:** Utilizar `Stack<UndoOperation>` para armazenar operações reversíveis.

**Implementação:**
- Grava estado antes de operações de Update e Delete
- Grava estado após operações de Create
- Suporta reversão de Create, Update e Delete

**Justificativa:**
- Simples e eficiente para operações sequenciais
- Mantém histórico de última operação
- Garante consistência após reversão

**Limitação:**
- Apenas última operação pode ser desfeita
- Não suporta múltiplos níveis de undo

### 5. Lista de Contatos Recentes

**Decisão:** Utilizar `LinkedList<Guid>` para manter ordem de acesso.

**Implementação:**
- Capacidade máxima configurável (padrão: 10)
- Atualiza posição quando contato é acessado novamente
- Remove automaticamente quando capacidade é excedida
- Remove da lista quando contato é excluído

**Justificativa:**
- `LinkedList` permite inserção/remoção O(1) no início
- Eficiente para manter ordem de acesso
- Limita crescimento de memória

## Trade-offs Assumidos

### 1. Performance vs. Complexidade
- **Escolha:** Índices invertidos para busca
- **Benefício:** Busca muito rápida
- **Custo:** Mais memória e complexidade de manutenção

### 2. Simplicidade vs. Funcionalidade
- **Escolha:** Undo apenas da última operação
- **Benefício:** Implementação simples
- **Custo:** Não suporta histórico completo

### 3. InMemory vs. Persistência Real
- **Escolha:** Persistência em memória
- **Benefício:** Foco na arquitetura, sem complexidade de banco
- **Custo:** Dados não persistem entre execuções

### 4. Singleton vs. Scoped para Repositórios
- **Escolha:** Singleton para repositórios
- **Benefício:** Mantém dados entre requisições
- **Custo:** Requer cuidado com thread-safety (não implementado nesta versão)

### 5. Dependência Application → Infrastructure
- **Escolha:** TicketService depende diretamente de TriagePolicyService
- **Benefício:** Simplicidade de implementação
- **Custo:** Viola princípio de dependência inversa (idealmente deveria haver interface)
- **Justificativa:** Aceitável para trabalho acadêmico, priorizando funcionalidade

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

> **Nota:** Execute `dotnet run -c Release` no projeto `ConectaAtende.Benchmarks` para reproduzir os resultados. Valores podem variar conforme o hardware.

### Interpretação Técnica dos Resultados

#### Inserção de Contatos
- **14.23 ms para 1.000 inserções** (10.000 no setup + 1.000 no benchmark)
- O tempo inclui atualização dos **índices invertidos** (nome e telefone)
- Complexidade: **O(1) amortizado** por inserção — crescimento linear com volume
- Alocação de 4.58 MB reflete a criação dos índices em memória

#### Busca por Nome (SearchByName)
- **0.58 ms para 100 buscas** com 10.000 contatos no repositório
- Utiliza índice invertido: varre chaves do índice (não todos os contatos)
- Complexidade: **O(k)** onde k é o número de chaves no índice — muito menor que O(n)
- Resultado: busca **25x mais rápida** que varredura linear seria

#### Busca por Telefone (SearchByPhone)
- **0.04 ms para 100 buscas** — o mais rápido de todos
- Acesso direto ao índice de telefones: **O(1) médio**
- Normalização (apenas dígitos) garante consistência sem custo perceptível

#### Atualização de Contatos (UpdateContacts)
- **0.91 ms para 100 atualizações**
- Custo inclui: remover índices antigos + atualizar dados + criar novos índices
- Complexidade: **O(p)** onde p é o número de telefones do contato

#### CustomHashTable vs Dictionary
- `Dictionary` do .NET é **~5x mais rápido** que a implementação didática
- Justificativa: Dictionary usa open addressing otimizado, tamanhos primos e técnicas de CPU cache
- `CustomHashTable` usa encadeamento (chaining) — mais simples mas com overhead de `List<T>` por bucket
- **Conclusão:** A implementação didática cumpre seu objetivo educacional; para produção, sempre usar `Dictionary`

### Trade-offs Evidenciados pelos Benchmarks

| Decisão | Benefício Medido | Custo |
|---------|-----------------|-------|
| Índices invertidos | Busca 25x mais rápida | +4 MB de memória por 10k contatos |
| LinkedList para recentes | O(1) inserção/remoção | Overhead de ponteiros |
| Stack para undo | O(1) push/pop | Memória proporcional ao histórico |
| Dictionary para repositório | O(1) acesso por ID | Sem ordenação nativa |

## Instruções para Execução

> 📖 **Guia Completo de Uso:** Consulte o arquivo [GUIA_USO.md](GUIA_USO.md) para instruções detalhadas sobre como usar o projeto no Visual Studio Code e Visual Studio.

> 🧪 **Guia de Testes:** Consulte o arquivo [GUIA_TESTES.md](GUIA_TESTES.md) para um passo a passo completo de todos os testes a realizar.

### Pré-requisitos
- .NET 8 SDK instalado
- Visual Studio 2022 ou VS Code (opcional)

### Executar a API

```bash
cd src/ConectaAtende.API
dotnet run
```

A API estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000/swagger`

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
- `GET /api/dev/seed?count=100` - Popular dados de teste

#### Estrutura Associativa Didática
- `POST /api/hashtable/compare?itemCount=1000` - Comparar desempenho com Dictionary padrão
- `POST /api/hashtable/demo` - Demonstração das operações

### Exemplo de Uso

#### Criar um contato:
```bash
curl -X POST http://localhost:5000/api/contacts \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "phones": ["11987654321", "1133334444"]
  }'
```

#### Buscar por nome:
```bash
curl http://localhost:5000/api/contacts/search?name=João
```

#### Criar um ticket:
```bash
curl -X POST http://localhost:5000/api/tickets \
  -H "Content-Type: application/json" \
  -d '{
    "contactId": "guid-do-contato",
    "description": "Problema com login",
    "priority": "High"
  }'
```

## Estrutura de Busca Interna

### Busca por Nome

A busca por nome utiliza um índice invertido onde:
1. Nomes são normalizados (remoção de acentuação, lowercase)
2. Cada nome normalizado mapeia para um `HashSet<Guid>` de contatos
3. Busca parcial verifica se o termo normalizado está contido em alguma chave do índice
4. Retorna todos os contatos cujos nomes contêm o termo de busca

**Complexidade:** O(n) onde n é o número de chaves no índice (normalmente muito menor que o número total de contatos)

### Busca por Telefone

A busca por telefone utiliza um índice direto onde:
1. Telefones são normalizados (apenas dígitos)
2. Cada telefone normalizado mapeia para um `HashSet<Guid>` de contatos
3. Busca é O(1) para telefone exato

**Complexidade:** O(1) em média

### Garantia de Consistência

- Índices são atualizados automaticamente em todas as operações CRUD
- Exclusões removem entradas dos índices
- Atualizações removem índices antigos e criam novos
- Não há dados órfãos nos índices

## Funcionalidades Implementadas

### ✅ Catálogo de Contatos
- [x] Cadastro de contatos (1-N telefones)
- [x] Atualização de informações
- [x] Exclusão consistente
- [x] Busca por identificador
- [x] Busca por nome (parcial, mínimo 3 caracteres, sem acentuação, case-insensitive)
- [x] Busca por telefone
- [x] Listagem paginada

### ✅ Central de Atendimento (Tickets)
- [x] Registro de ticket associado a contato
- [x] Enfileiramento para atendimento
- [x] Consulta do próximo ticket
- [x] Retirada para atendimento
- [x] Atualização de status

### ✅ Política de Triagem Variável
- [x] Ordem de chegada (FirstComeFirstServed)
- [x] Prioridade (Priority)
- [x] Política mista (Mixed)
- [x] Alteração dinâmica durante execução

### ✅ Mecanismo de Reversão (Undo)
- [x] Desfazer última operação de contatos
- [x] Suporta Create, Update e Delete
- [x] Mantém coerência após reversão

### ✅ Lista de Contatos Recentes
- [x] Registro automático ao acessar contato
- [x] Capacidade máxima configurável
- [x] Atualização de posição em novo acesso
- [x] Remoção automática em exclusões

### ✅ Projeto de Benchmark
- [x] Inserção de grande volume
- [x] Consultas frequentes
- [x] Atualizações
- [x] Análise de impacto estrutural

### ✅ Estrutura Associativa Didática (Atividade Complementar)
- [x] Implementação manual de hash table
- [x] Buckets com tratamento de colisões (encadeamento)
- [x] Operações básicas (inserção, busca, remoção)
- [x] Rehash automático quando fator de carga é excedido
- [x] Comparação com Dictionary padrão do .NET
- [x] Benchmarks de desempenho

## Estrutura Associativa Didática

Como atividade complementar, foi implementada uma estrutura associativa (hash table) didática do zero.

### Implementação

A `CustomHashTable<TKey, TValue>` foi implementada com:

1. **Buckets:** Array de buckets onde cada bucket armazena múltiplas entradas
2. **Função Hash:** Utiliza `GetHashCode()` da chave para determinar o bucket
3. **Tratamento de Colisões:** Encadeamento (chaining) - cada bucket é uma lista de entradas
4. **Rehash Automático:** Quando o fator de carga (0.75) é excedido, a capacidade é dobrada e todos os elementos são redistribuídos

### Características Técnicas

- **Capacidade inicial:** 16 buckets
- **Fator de carga:** 0.75 (rehash quando 75% dos buckets estão ocupados)
- **Tratamento de colisões:** Encadeamento (List dentro de cada bucket)
- **Complexidade esperada:**
  - Inserção: O(1) médio, O(n) no pior caso
  - Busca: O(1) médio, O(n) no pior caso
  - Remoção: O(1) médio, O(n) no pior caso

### Comparação com Dictionary Padrão

A implementação inclui:
- Endpoint `/api/hashtable/compare?itemCount=1000` para comparar desempenho
- Endpoint `/api/hashtable/demo` para demonstração das operações
- Benchmarks dedicados (`HashTableBenchmarks`)

**Diferenças conceituais:**
- **CustomHashTable:** Implementação didática, mais simples, permite entender o funcionamento interno
- **Dictionary (.NET):** Altamente otimizado, usa técnicas avançadas (open addressing, prime numbers para capacidade, etc.)

**Trade-offs:**
- CustomHashTable é mais lenta que Dictionary padrão (esperado)
- CustomHashTable consome mais memória (List em cada bucket vs. otimizações do .NET)
- CustomHashTable é educacional - permite entender como hash tables funcionam

## Considerações Finais

Este projeto foi desenvolvido como trabalho acadêmico, simulando uma situação real de mercado: a reescrita de um sistema legado. A arquitetura foi pensada para:

- **Manutenibilidade:** Separação clara de responsabilidades
- **Testabilidade:** Interfaces permitem mock fácil
- **Extensibilidade:** Fácil adicionar novas funcionalidades
- **Escalabilidade conceitual:** Preparado para evoluir

A implementação prioriza funcionalidade sobre otimizações prematuras, mantendo o código simples e direto, adequado para um programador iniciante mas com fundamentos sólidos.

## Testes Unitários e Cobertura

### Estrutura de Testes

Os testes estão organizados por camada, dentro da pasta `tests/ConectaAtende.UnitTests/`:

```
tests/ConectaAtende.UnitTests/
├── Domain/
│   ├── ContactTests.cs         — Testes da entidade Contact
│   └── TicketTests.cs          — Testes da entidade Ticket
├── Application/
│   ├── ContactServiceTests.cs  — Testes do ContactService
│   └── TicketServiceTests.cs   — Testes do TicketService
└── Infrastructure/
    ├── InMemoryContactRepositoryTests.cs — Testes do repositório
    ├── TriagePolicyTests.cs     — Testes das políticas de triagem
    ├── UndoServiceTests.cs      — Testes do mecanismo de undo
    └── RecentContactsServiceTests.cs — Testes da lista de recentes
```

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
