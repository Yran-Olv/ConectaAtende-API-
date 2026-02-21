# 🔧 Correção: Erro ao Criar Contato no Swagger

## ❌ Erro Encontrado

Ao tentar criar um contato via Swagger, você recebeu:
```
400 Bad Request
"'`' is an invalid start of a value."
```

## 🔍 Causa do Problema

O Swagger está recebendo o JSON com os **backticks (```)** e a palavra **"json"** incluídos, o que torna o JSON inválido.

## ✅ Solução

### No Swagger, cole APENAS o JSON puro:

**❌ ERRADO (não cole isso):**
```
```json
{
  "name": "João Silva",
  "phones": ["11987654321", "1133334444"]
}
```
```

**✅ CORRETO (cole apenas isso):**
```json
{
  "name": "João Silva",
  "phones": ["11987654321", "1133334444"]
}
```

### Passo a Passo Correto:

1. Acesse `http://localhost:5000/swagger`
2. Expanda `POST /api/contacts`
3. Clique em **Try it out**
4. No campo **Request body**, cole **APENAS**:
   ```json
   {
     "name": "João Silva",
     "phones": ["11987654321", "1133334444"]
   }
   ```
5. Clique em **Execute**

## 📝 Exemplos de JSON Corretos

### Criar Contato Simples:
```json
{
  "name": "João Silva",
  "phones": ["11987654321"]
}
```

### Criar Contato com Múltiplos Telefones:
```json
{
  "name": "Maria Santos",
  "phones": ["11987654321", "1133334444", "1199887766"]
}
```

### Criar Contato (sem telefones):
```json
{
  "name": "Pedro Oliveira",
  "phones": []
}
```

## 🧪 Teste Rápido

Execute este comando no PowerShell para verificar se funciona:

```powershell
$body = @{
    name = "João Silva"
    phones = @("11987654321", "1133334444")
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/contacts" -Method POST -Body $body -ContentType "application/json"
```

Se funcionar no PowerShell, o problema era apenas a formatação no Swagger.

## 💡 Dica

No Swagger, o campo **Request body** já está configurado para `application/json`, então você só precisa colar o JSON puro, sem nenhuma formatação adicional.

---

**Agora tente novamente com o JSON correto!** ✅
