# 🧪 Guia de Testes - ConectaAtende API

Este guia apresenta um passo a passo completo para testar todas as funcionalidades da API ConectaAtende.

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de que:

- [ ] A API está rodando (`dotnet run` no projeto `ConectaAtende.API`)
- [ ] A API está acessível em `http://localhost:5000`
- [ ] O Swagger está disponível em `http://localhost:5000/swagger`

---

## 🚀 Início Rápido

### 1. Verificar se a API está rodando

**No navegador:**
```
http://localhost:5000/swagger
```

**Ou via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method GET
```

Se retornar uma lista (mesmo que vazia), a API está funcionando! ✅

---

## 📝 TESTE 1: Catálogo de Contatos

### 1.1 Criar um Contato

**Via Swagger:**
1. Acesse `http://localhost:5000/swagger`
2. Expanda `POST /api/contacts`
3. Clique em **Try it out**
4. **IMPORTANTE:** Cole APENAS o JSON abaixo (SEM os backticks ``` ou a palavra json):
```json
{
  "name": "João Silva",
  "phones": ["11987654321", "1133334444"]
}
```
   **Cole apenas isso no campo Request body:**
   ```
   {
     "name": "João Silva",
     "phones": ["11987654321", "1133334444"]
   }
   ```
5. Clique em **Execute**
6. Verifique se retorna status `201 Created` com os dados do contato

**⚠️ ATENÇÃO:** Não cole os backticks (```) ou a palavra "json" - apenas o conteúdo JSON puro!

**Via PowerShell:**
```powershell
$body = @{
    name = "João Silva"
    phones = @("11987654321", "1133334444")
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method POST -Body $body -ContentType "application/json"
$response
```

**✅ Resultado esperado:**
- Status: `201 Created`
- Retorna o contato criado com `id`, `name`, `phones`, `createdAt`, `updatedAt`

**💾 Guarde o `id` do contato criado para os próximos testes!**

---

### 1.2 Buscar Contato por ID

**Via Swagger:**
1. Expanda `GET /api/contacts/{id}`
2. Clique em **Try it out**
3. Cole o `id` do contato criado anteriormente
4. Clique em **Execute**

**Via PowerShell:**
```powershell
# Substitua CONTACT_ID pelo ID do contato criado
$contactId = "CONTACT_ID"
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna os dados completos do contato

---

### 1.3 Listar Contatos (Paginação)

**Via Swagger:**
1. Expanda `GET /api/contacts`
2. Clique em **Try it out**
3. Defina `page = 1` e `pageSize = 10`
4. Clique em **Execute**

**Via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts?page=1&pageSize=10" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna objeto com:
  - `items`: Lista de contatos
  - `page`: Página atual
  - `pageSize`: Tamanho da página
  - `totalCount`: Total de contatos
  - `totalPages`: Total de páginas

---

### 1.4 Buscar Contato por Nome

**Via Swagger:**
1. Expanda `GET /api/contacts/search`
2. Clique em **Try it out**
3. Defina `name = "João"` (mínimo 3 caracteres)
4. Clique em **Execute**

**Via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?name=João" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna lista de contatos cujo nome contém "João"
- Deve ignorar acentuação e ser case-insensitive

**🧪 Teste adicional:**
```powershell
# Teste sem acentuação
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?name=Joao" -Method GET

# Teste com menos de 3 caracteres (deve retornar vazio)
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?name=Jo" -Method GET
```

---

### 1.5 Buscar Contato por Telefone

**Via Swagger:**
1. Expanda `GET /api/contacts/search`
2. Clique em **Try it out**
3. Defina `phone = "11987654321"`
4. Clique em **Execute**

**Via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?phone=11987654321" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna lista de contatos com esse telefone
- Deve funcionar mesmo se o telefone foi cadastrado com formatação diferente

**🧪 Teste adicional:**
```powershell
# Teste com formatação diferente (deve normalizar)
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?phone=(11)98765-4321" -Method GET
```

---

### 1.6 Atualizar Contato

**Via Swagger:**
1. Expanda `PUT /api/contacts/{id}`
2. Clique em **Try it out**
3. Cole o `id` do contato
4. Cole o JSON:
```json
{
  "name": "João Silva Santos",
  "phones": ["11987654321", "11999999999"]
}
```
5. Clique em **Execute**

**Via PowerShell:**
```powershell
$contactId = "CONTACT_ID"
$body = @{
    name = "João Silva Santos"
    phones = @("11987654321", "11999999999")
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId" -Method PUT -Body $body -ContentType "application/json"
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna o contato atualizado
- `updatedAt` deve ser atualizado

**🧪 Verifique:**
- Busque o contato novamente e confirme que os dados foram atualizados

---

### 1.7 Excluir Contato

**Via Swagger:**
1. Expanda `DELETE /api/contacts/{id}`
2. Clique em **Try it out**
3. Cole o `id` do contato
4. Clique em **Execute**

**Via PowerShell:**
```powershell
$contactId = "CONTACT_ID"
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId" -Method DELETE
```

**✅ Resultado esperado:**
- Status: `204 No Content`
- Contato removido

**🧪 Verifique:**
- Tente buscar o contato novamente - deve retornar `404 Not Found`
- Tente buscar por telefone - não deve aparecer nos resultados

---

## 🔄 TESTE 2: Mecanismo de Undo

### 2.1 Desfazer Criação

**Passos:**
1. Crie um novo contato (guarde o `id`)
2. Execute o undo:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/undo" -Method POST
```
3. Verifique se o contato foi removido:
```powershell
# Deve retornar 404
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/CONTACT_ID" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK` com mensagem de sucesso
- Contato criado foi removido

---

### 2.2 Desfazer Atualização

**Passos:**
1. Crie um contato
2. Atualize o contato (mude o nome)
3. Execute o undo:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/undo" -Method POST
```
4. Verifique se o contato voltou ao estado anterior:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/CONTACT_ID" -Method GET
```

**✅ Resultado esperado:**
- Nome voltou ao valor anterior à atualização

---

### 2.3 Desfazer Exclusão

**Passos:**
1. Crie um contato (guarde o `id`)
2. Exclua o contato
3. Execute o undo:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/undo" -Method POST
```
4. Verifique se o contato foi restaurado:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/CONTACT_ID" -Method GET
```

**✅ Resultado esperado:**
- Contato foi restaurado com todos os dados originais

---

### 2.4 Tentar Undo sem Operação

**Via PowerShell:**
```powershell
# Execute undo duas vezes seguidas (sem fazer operação entre elas)
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/undo" -Method POST
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/undo" -Method POST
```

**✅ Resultado esperado:**
- Primeira chamada: `200 OK`
- Segunda chamada: `400 Bad Request` com mensagem "Não há operação para desfazer"

---

## 📋 TESTE 3: Lista de Contatos Recentes

### 3.1 Acessar Contato e Verificar Recentes

**Passos:**
1. Crie 3 contatos diferentes (guarde os IDs)
2. Acesse cada contato pelo ID:
```powershell
$contactId1 = "ID_1"
$contactId2 = "ID_2"
$contactId3 = "ID_3"

# Acesse na ordem: 1, 2, 3
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId1" -Method GET
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId2" -Method GET
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId3" -Method GET
```
3. Liste os recentes:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/recent?limit=10" -Method GET
```

**✅ Resultado esperado:**
- Lista deve estar na ordem: 3, 2, 1 (mais recente primeiro)

---

### 3.2 Acessar Contato Novamente

**Passos:**
1. Acesse o contato 1 novamente:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId1" -Method GET
```
2. Liste os recentes novamente:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/recent?limit=10" -Method GET
```

**✅ Resultado esperado:**
- Contato 1 deve estar no topo da lista
- Não deve haver duplicatas

---

### 3.3 Excluir Contato e Verificar Recentes

**Passos:**
1. Exclua um contato que está na lista de recentes:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId2" -Method DELETE
```
2. Liste os recentes:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/recent?limit=10" -Method GET
```

**✅ Resultado esperado:**
- Contato excluído não deve aparecer na lista de recentes

---

## 🎫 TESTE 4: Central de Atendimento (Tickets)

### 4.1 Criar um Ticket

**Pré-requisito:** Você precisa de um `contactId` válido.

**Via Swagger:**
1. Expanda `POST /api/tickets`
2. Clique em **Try it out**
3. Cole o JSON:
```json
{
  "contactId": "CONTACT_ID",
  "description": "Problema com login no sistema",
  "priority": "High"
}
```
4. Clique em **Execute**

**Via PowerShell:**
```powershell
$contactId = "CONTACT_ID" # Use um ID válido
$body = @{
    contactId = $contactId
    description = "Problema com login no sistema"
    priority = "High"
} | ConvertTo-Json

$ticket = Invoke-RestMethod -Uri "http://localhost:5000/api/tickets" -Method POST -Body $body -ContentType "application/json"
$ticket
```

**✅ Resultado esperado:**
- Status: `201 Created`
- Retorna ticket com:
  - `id`
  - `contactId`
  - `description`
  - `status`: "Created"
  - `priority`: "High"
  - `createdAt`

**💾 Guarde o `id` do ticket para os próximos testes!**

**🧪 Teste de erro:**
```powershell
# Tente criar ticket com contactId inválido
$body = @{
    contactId = "00000000-0000-0000-0000-000000000000"
    description = "Teste"
    priority = "Medium"
} | ConvertTo-Json

# Deve retornar 400 Bad Request
try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/tickets" -Method POST -Body $body -ContentType "application/json"
} catch {
    $_.Exception.Response.StatusCode
}
```

---

### 4.2 Buscar Ticket por ID

**Via PowerShell:**
```powershell
$ticketId = "TICKET_ID"
Invoke-RestMethod -Uri "http://localhost:5000/api/tickets/$ticketId" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna dados completos do ticket

---

### 4.3 Enfileirar Ticket

**Via Swagger:**
1. Expanda `POST /api/tickets/enqueue/{ticketId}`
2. Clique em **Try it out**
3. Cole o `ticketId`
4. Clique em **Execute**

**Via PowerShell:**
```powershell
$ticketId = "TICKET_ID"
$ticket = Invoke-RestMethod -Uri "http://localhost:5000/api/tickets/enqueue/$ticketId" -Method POST
$ticket
```

**✅ Resultado esperado:**
- Status: `200 OK`
- `status` mudou para "Queued"
- `queuedAt` foi preenchido

**🧪 Teste de erro:**
```powershell
# Tente enfileirar o mesmo ticket duas vezes
# Segunda tentativa deve retornar erro (ticket já está enfileirado)
```

---

### 4.4 Obter Próximo Ticket (Sem Retirar)

**Via Swagger:**
1. Expanda `GET /api/tickets/next`
2. Clique em **Try it out**
3. Clique em **Execute**

**Via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/tickets/next" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK` ou `404 Not Found` (se não houver tickets na fila)
- Retorna o próximo ticket conforme a política de triagem atual
- **Não altera o status** do ticket (apenas consulta)

---

### 4.5 Retirar Ticket da Fila (Dequeue)

**Via Swagger:**
1. Expanda `POST /api/tickets/dequeue`
2. Clique em **Try it out**
3. Clique em **Execute**

**Via PowerShell:**
```powershell
$ticket = Invoke-RestMethod -Uri "http://localhost:5000/api/tickets/dequeue" -Method POST
$ticket
```

**✅ Resultado esperado:**
- Status: `200 OK`
- `status` mudou para "InProgress"
- `dequeuedAt` foi preenchido
- Ticket foi removido da fila

**🧪 Teste:**
```powershell
# Crie múltiplos tickets e enfileire-os
# Execute dequeue e verifique se o ticket correto foi retirado conforme a política
```

---

## ⚙️ TESTE 5: Política de Triagem

### 5.1 Verificar Política Atual

**Via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/triage/policy" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna: `{ "policy": "FirstComeFirstServed" }` (padrão)

---

### 5.2 Alterar para Política de Prioridade

**Via Swagger:**
1. Expanda `POST /api/triage/policy`
2. Clique em **Try it out**
3. Cole o JSON:
```json
{
  "policy": "Priority"
}
```
4. Clique em **Execute**

**Via PowerShell:**
```powershell
$body = @{
    policy = "Priority"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/triage/policy" -Method POST -Body $body -ContentType "application/json"
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Mensagem confirmando alteração

**🧪 Teste:**
1. Crie 3 tickets com prioridades diferentes:
   - Ticket 1: Low (criado primeiro)
   - Ticket 2: High (criado segundo)
   - Ticket 3: Medium (criado terceiro)
2. Enfileire todos
3. Execute `GET /api/tickets/next`
4. **Resultado esperado:** Ticket 2 (High) deve ser o próximo, mesmo tendo sido criado depois

---

### 5.3 Alterar para Política Mista

**Via PowerShell:**
```powershell
$body = @{
    policy = "Mixed"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/triage/policy" -Method POST -Body $body -ContentType "application/json"
```

**✅ Resultado esperado:**
- Status: `200 OK`

**🧪 Teste:**
- Crie tickets com diferentes prioridades
- Verifique se a política mista prioriza High, depois Medium, depois Low, e dentro de cada prioridade usa ordem de chegada

---

### 5.4 Alterar para First Come First Served

**Via PowerShell:**
```powershell
$body = @{
    policy = "FirstComeFirstServed"
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/triage/policy" -Method POST -Body $body -ContentType "application/json"
```

**✅ Resultado esperado:**
- Status: `200 OK`

**🧪 Teste:**
- Crie múltiplos tickets e enfileire-os
- Verifique se são atendidos na ordem de criação (FIFO)

---

### 5.5 Teste de Política Inválida

**Via PowerShell:**
```powershell
$body = @{
    policy = "InvalidPolicy"
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/triage/policy" -Method POST -Body $body -ContentType "application/json"
} catch {
    $_.Exception.Response.StatusCode # Deve ser 400
}
```

**✅ Resultado esperado:**
- Status: `400 Bad Request`
- Mensagem de erro informando que a política não foi encontrada

---

## 🗂️ TESTE 6: Estrutura Associativa Didática

### 6.1 Demonstração das Operações

**Via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/hashtable/demo" -Method POST
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna objeto mostrando:
  - Inserção de chaves
  - Busca de valores
  - Remoção de chaves
  - Estado antes e depois da remoção

---

### 6.2 Comparação com Dictionary Padrão

**Via PowerShell:**
```powershell
# Compare com 1000 itens
Invoke-RestMethod -Uri "http://localhost:5000/api/hashtable/compare?itemCount=1000" -Method POST
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Retorna métricas de desempenho comparando:
  - `CustomHashTable` vs `StandardDictionary`
  - Tempos de inserção, busca e remoção
  - Capacidade final

**🧪 Teste com diferentes volumes:**
```powershell
# Teste com 100 itens
Invoke-RestMethod -Uri "http://localhost:5000/api/hashtable/compare?itemCount=100" -Method POST

# Teste com 10000 itens
Invoke-RestMethod -Uri "http://localhost:5000/api/hashtable/compare?itemCount=10000" -Method POST
```

---

## 🧹 TESTE 7: Endpoint de Desenvolvimento (Seed)

### 7.1 Popular Dados de Teste

**Via Swagger:**
1. Expanda `GET /api/dev/seed`
2. Clique em **Try it out**
3. Defina `count = 100`
4. Clique em **Execute**

**Via PowerShell:**
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/dev/seed?count=100" -Method GET
```

**✅ Resultado esperado:**
- Status: `200 OK`
- Mensagem: "100 contatos criados com sucesso"

**🧪 Verifique:**
```powershell
# Liste os contatos
$result = Invoke-RestMethod -Uri "http://localhost:5000/api/contacts?page=1&pageSize=10" -Method GET
$result.totalCount # Deve ser pelo menos 100
```

---

### 7.2 Testar Busca com Grande Volume

**Após popular 100 contatos:**
```powershell
# Busque por nome comum
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?name=João" -Method GET

# Verifique o tempo de resposta (deve ser rápido devido aos índices)
```

**✅ Resultado esperado:**
- Busca deve ser rápida mesmo com muitos contatos
- Resultados corretos

---

## 📊 Checklist Completo de Testes

Use este checklist para garantir que testou tudo:

### Catálogo de Contatos
- [ ] Criar contato com múltiplos telefones
- [ ] Buscar contato por ID
- [ ] Listar contatos paginados
- [ ] Buscar por nome (mínimo 3 caracteres)
- [ ] Buscar por nome sem acentuação
- [ ] Buscar por telefone
- [ ] Buscar por telefone com formatação diferente
- [ ] Atualizar contato
- [ ] Excluir contato
- [ ] Verificar que exclusão remove das buscas

### Mecanismo de Undo
- [ ] Desfazer criação
- [ ] Desfazer atualização
- [ ] Desfazer exclusão
- [ ] Tentar undo sem operação (deve retornar erro)

### Lista de Recentes
- [ ] Acessar contato e verificar se aparece nos recentes
- [ ] Acessar contato novamente e verificar se sobe para o topo
- [ ] Verificar ordem dos recentes (mais recente primeiro)
- [ ] Excluir contato e verificar se sai dos recentes

### Central de Atendimento
- [ ] Criar ticket
- [ ] Criar ticket com contato inválido (deve retornar erro)
- [ ] Buscar ticket por ID
- [ ] Enfileirar ticket
- [ ] Tentar enfileirar ticket duas vezes (deve retornar erro)
- [ ] Obter próximo ticket (sem retirar)
- [ ] Retirar ticket da fila
- [ ] Verificar mudança de status após dequeue

### Política de Triagem
- [ ] Verificar política padrão
- [ ] Alterar para Priority
- [ ] Alterar para Mixed
- [ ] Alterar para FirstComeFirstServed
- [ ] Testar política inválida (deve retornar erro)
- [ ] Verificar que tickets são selecionados conforme política

### Estrutura Associativa
- [ ] Executar demonstração
- [ ] Comparar desempenho com Dictionary padrão

### Endpoint de Desenvolvimento
- [ ] Popular dados de teste
- [ ] Verificar que contatos foram criados
- [ ] Testar busca com grande volume

---

## 🐛 Solução de Problemas

### Erro 404 em todos os endpoints
- Verifique se a API está rodando
- Verifique a URL (deve ser `http://localhost:5000`)

### Erro 500 Internal Server Error
- Verifique os logs no console onde a API está rodando
- Pode ser problema de dependências - execute `dotnet restore`

### Swagger não abre
- Verifique se está em modo Development
- Tente acessar diretamente: `http://localhost:5000/swagger/index.html`

### Contatos não aparecem após criar
- Verifique se está usando o mesmo repositório (Singleton)
- Reinicie a API se necessário

---

## ✅ Conclusão

Após completar todos os testes acima, você terá verificado que:

- ✅ Todos os endpoints estão funcionando
- ✅ Validações estão corretas
- ✅ Políticas de triagem funcionam
- ✅ Undo funciona corretamente
- ✅ Buscas estão otimizadas
- ✅ Lista de recentes está funcionando

**Se todos os testes passaram, a API está funcionando perfeitamente!** 🎉

---

**Dica:** Salve este arquivo e use como referência durante o desenvolvimento e testes!
