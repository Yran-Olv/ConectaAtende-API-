# ⚡ Teste Rápido - Verificar se a API está Funcionando

## ✅ Status Atual

Os endpoints **ESTÃO FUNCIONANDO**! O teste confirmou que a API está respondendo corretamente.

---

## 🧪 Teste Rápido via PowerShell

### 1. Testar Listagem de Contatos

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method GET
```

**✅ Se funcionar:** Você verá um objeto JSON com `items`, `page`, `pageSize`, etc.

---

### 2. Testar Criação de Contato

```powershell
$body = @{
    name = "Teste API"
    phones = @("11999999999")
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method POST -Body $body -ContentType "application/json"
```

**✅ Se funcionar:** Você verá o contato criado com um `id` gerado.

---

### 3. Testar Busca por Nome

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?name=Teste" -Method GET
```

**✅ Se funcionar:** Você verá uma lista de contatos que contêm "Teste" no nome.

---

## 🌐 Teste via Navegador

### URLs que Funcionam no Navegador:

1. **Swagger UI:**
   ```
   http://localhost:5000/swagger
   ```

2. **Endpoints GET (funcionam no navegador):**
   ```
   http://localhost:5000/api/contacts
   http://localhost:5000/api/contacts?page=1&pageSize=10
   http://localhost:5000/api/contacts/search?name=João
   http://localhost:5000/api/triage/policy
   ```

### URLs que NÃO Funcionam no Navegador (precisam de POST/PUT/DELETE):

- ❌ `POST /api/contacts` - Precisa de body JSON
- ❌ `PUT /api/contacts/{id}` - Precisa de body JSON
- ❌ `DELETE /api/contacts/{id}` - Precisa de método DELETE
- ❌ `POST /api/tickets` - Precisa de body JSON

**💡 Solução:** Use o Swagger para testar esses endpoints, ou use PowerShell/Postman.

---

## 🔍 Verificar se Endpoints Estão Registrados

### Via Swagger

1. Acesse: `http://localhost:5000/swagger`
2. Você deve ver todos os controllers:
   - **Contacts** - Endpoints de contatos
   - **Tickets** - Endpoints de tickets
   - **Triage** - Endpoints de política de triagem
   - **HashTable** - Endpoints da estrutura associativa
   - **Dev** - Endpoint de seed

### Lista de Endpoints Disponíveis

#### Contatos
- ✅ `GET /api/contacts` - Listar (funciona no navegador)
- ✅ `GET /api/contacts/{id}` - Buscar por ID (funciona no navegador)
- ✅ `GET /api/contacts/search?name=...` - Buscar por nome (funciona no navegador)
- ✅ `GET /api/contacts/search?phone=...` - Buscar por telefone (funciona no navegador)
- ✅ `GET /api/contacts/recent` - Listar recentes (funciona no navegador)
- ⚠️ `POST /api/contacts` - Criar (precisa Swagger/PowerShell)
- ⚠️ `PUT /api/contacts/{id}` - Atualizar (precisa Swagger/PowerShell)
- ⚠️ `DELETE /api/contacts/{id}` - Excluir (precisa Swagger/PowerShell)
- ⚠️ `POST /api/contacts/undo` - Undo (precisa Swagger/PowerShell)

#### Tickets
- ✅ `GET /api/tickets/{id}` - Buscar por ID (funciona no navegador)
- ⚠️ `POST /api/tickets` - Criar (precisa Swagger/PowerShell)
- ⚠️ `POST /api/tickets/enqueue/{id}` - Enfileirar (precisa Swagger/PowerShell)
- ✅ `GET /api/tickets/next` - Próximo ticket (funciona no navegador)
- ⚠️ `POST /api/tickets/dequeue` - Retirar da fila (precisa Swagger/PowerShell)

#### Triagem
- ✅ `GET /api/triage/policy` - Ver política (funciona no navegador)
- ⚠️ `POST /api/triage/policy` - Alterar política (precisa Swagger/PowerShell)

---

## 🐛 Problemas Comuns

### "Não consigo acessar os endpoints no navegador"

**Causa:** Navegadores só fazem requisições GET por padrão. POST, PUT, DELETE precisam de ferramentas especiais.

**Solução:**
1. Use o **Swagger** (`http://localhost:5000/swagger`) - Interface visual para testar todos os endpoints
2. Use **PowerShell** (veja exemplos acima)
3. Use **Postman** ou **Insomnia**

---

### "Swagger abre mas não mostra os endpoints"

**Possíveis causas:**
1. Controllers não estão sendo registrados
2. Erro de compilação

**Solução:**
```powershell
# Verificar se compila
cd "C:\Users\yran\Desktop\ConectaAtende API (.NET 8)"
& "C:\Program Files\dotnet\dotnet.exe" build

# Se houver erros, corrija-os
# Se compilar com sucesso, reinicie a API
```

---

### "Erro 404 em todos os endpoints"

**Causa:** API não está rodando ou roteamento incorreto.

**Solução:**
1. Verifique se a API está rodando:
   ```powershell
   netstat -ano | findstr :5000
   ```
2. Verifique se está usando a rota correta: `/api/contacts` (não `/contacts`)
3. Reinicie a API se necessário

---

### "Erro 500 Internal Server Error"

**Causa:** Erro no código da API.

**Solução:**
1. Verifique os logs no console onde a API está rodando
2. Procure por mensagens de erro
3. Verifique se todas as dependências estão instaladas:
   ```powershell
   dotnet restore
   ```

---

## ✅ Checklist de Verificação

Use este checklist para verificar se tudo está funcionando:

- [ ] API está rodando (porta 5000 ativa)
- [ ] Swagger abre em `http://localhost:5000/swagger`
- [ ] Swagger mostra todos os controllers
- [ ] `GET /api/contacts` funciona no navegador
- [ ] `GET /api/contacts` funciona no PowerShell
- [ ] `POST /api/contacts` funciona no Swagger
- [ ] `POST /api/contacts` funciona no PowerShell
- [ ] Busca por nome funciona
- [ ] Busca por telefone funciona
- [ ] Tickets podem ser criados
- [ ] Política de triagem pode ser alterada

---

## 🎯 Teste Completo em 5 Minutos

Execute estes comandos em sequência no PowerShell:

```powershell
# 1. Listar contatos (deve funcionar)
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method GET

# 2. Criar contato
$body = @{name="Teste Completo";phones=@("11999999999")} | ConvertTo-Json
$contact = Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method POST -Body $body -ContentType "application/json"
$contactId = $contact.id

# 3. Buscar contato criado
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/$contactId" -Method GET

# 4. Buscar por nome
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts/search?name=Teste" -Method GET

# 5. Ver política de triagem
Invoke-RestMethod -Uri "http://localhost:5000/api/triage/policy" -Method GET

# 6. Popular dados de teste
Invoke-RestMethod -Uri "http://localhost:5000/api/dev/seed?count=5" -Method GET

# 7. Listar novamente (deve ter mais contatos)
Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method GET
```

**✅ Se todos os comandos executarem sem erro, a API está 100% funcional!**

---

## 📝 Nota Importante

**Endpoints GET funcionam no navegador**, mas **POST, PUT, DELETE precisam de ferramentas especiais** como:
- Swagger UI (recomendado - já está configurado)
- PowerShell (exemplos acima)
- Postman
- Insomnia
- cURL

O Swagger é a forma mais fácil de testar todos os endpoints visualmente!

---

## 🎉 Conclusão

Se você consegue:
- ✅ Acessar o Swagger
- ✅ Ver os endpoints listados
- ✅ Executar pelo menos um GET com sucesso

**Então a API está funcionando perfeitamente!** 

O fato de apenas o Swagger estar "acessível" no navegador é normal - os outros endpoints precisam ser testados através do Swagger ou de ferramentas de API.
