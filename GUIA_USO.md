# 🚀 Guia de Uso - ConectaAtende API (.NET 8)

Este guia explica como configurar, executar e usar o projeto ConectaAtende API nas principais IDEs e editores de código.

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

- **.NET 8 SDK** - [Download aqui](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git** (opcional, para clonar o repositório)
- **Visual Studio 2022** ou **Visual Studio Code** (escolha uma das opções abaixo)

### Verificando a Instalação

Abra um terminal (PowerShell, CMD ou Terminal) e execute:

```bash
dotnet --version
```

Você deve ver algo como: `8.0.xxx`

---

## 🎯 Opção 1: Visual Studio Code

### 1.1 Instalação e Configuração

1. **Instale o Visual Studio Code**
   - Download: [https://code.visualstudio.com/](https://code.visualstudio.com/)

2. **Instale as Extensões Necessárias**
   - Abra o VS Code
   - Pressione `Ctrl+Shift+X` (ou `Cmd+Shift+X` no Mac) para abrir a aba de extensões
   - Instale as seguintes extensões:
     - **C#** (Microsoft) - Extensão oficial do C#
     - **C# Dev Kit** (Microsoft) - Kit de desenvolvimento completo
     - **.NET Extension Pack** (Microsoft) - Pacote com várias extensões úteis

3. **Abrir o Projeto**
   ```bash
   # Navegue até a pasta do projeto
   cd "C:\Users\yran\Desktop\ConectaAtende API (.NET 8)"
   
   # Abra no VS Code
   code .
   ```
   
   Ou pelo VS Code:
   - `File` → `Open Folder...`
   - Selecione a pasta do projeto

### 1.2 Restaurar Dependências

No terminal integrado do VS Code (`Ctrl+`` ou `View` → `Terminal`):

```bash
# Restaurar pacotes NuGet
dotnet restore
```

### 1.3 Executar a API

**Método 1: Terminal Integrado**

```bash
# Navegue até o projeto da API
cd src/ConectaAtende.API

# Execute a API
dotnet run
```

**Método 2: Usando Tasks do VS Code**

1. Pressione `Ctrl+Shift+P` (ou `Cmd+Shift+P` no Mac)
2. Digite: `Tasks: Run Task`
3. Selecione: `build` ou `run`

**Método 3: Botão de Play**

- Abra o arquivo `src/ConectaAtende.API/Program.cs`
- Clique no botão "▶ Run" ou "▶ Debug" acima do método `Main`

### 1.4 Acessar a API

Após executar, você verá algo como:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

- **Swagger UI:** [http://localhost:5000/swagger](http://localhost:5000/swagger)
- **API Base:** [http://localhost:5000/api](http://localhost:5000/api)

### 1.5 Executar Benchmarks

```bash
# Navegue até o projeto de benchmarks
cd src/ConectaAtende.Benchmarks

# Execute em modo Release (necessário para benchmarks precisos)
dotnet run -c Release
```

### 1.6 Debug no VS Code

1. Abra o arquivo que deseja debugar
2. Coloque um breakpoint (clique na margem esquerda ou `F9`)
3. Pressione `F5` ou clique em "▶ Debug"
4. Selecione ".NET Core" quando solicitado
5. O debugger será iniciado

**Arquivos de configuração já incluídos:**
- `.vscode/launch.json` - Configurações de debug
- `.vscode/tasks.json` - Tarefas de build e execução
- `.vscode/settings.json` - Configurações do editor

Esses arquivos já estão no projeto e configuram automaticamente o debug e as tarefas!

---

## 🎯 Opção 2: Visual Studio 2022

### 2.1 Instalação e Configuração

1. **Instale o Visual Studio 2022**
   - Download: [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/)
   - Durante a instalação, selecione a carga de trabalho:
     - **ASP.NET e desenvolvimento Web**
     - **Desenvolvimento para desktop com .NET**

2. **Abrir o Projeto**
   - Abra o Visual Studio 2022
   - `File` → `Open` → `Project/Solution...`
   - Navegue até a pasta do projeto
   - Selecione `ConectaAtende.sln`

### 2.2 Restaurar Dependências

O Visual Studio geralmente restaura automaticamente. Se não:

1. Clique com o botão direito na **Solution** no **Solution Explorer**
2. Selecione **Restore NuGet Packages**

Ou pelo menu:
- `Tools` → `NuGet Package Manager` → `Package Manager Console`
- Execute: `dotnet restore`

### 2.3 Executar a API

**Método 1: Executar Diretamente**

1. No **Solution Explorer**, clique com o botão direito em `ConectaAtende.API`
2. Selecione **Set as Startup Project**
3. Pressione `F5` (Debug) ou `Ctrl+F5` (Executar sem debug)

**Método 2: Usando o Menu**

- `Debug` → `Start Debugging` (F5)
- `Debug` → `Start Without Debugging` (Ctrl+F5)

### 2.4 Acessar a API

Após executar, o navegador padrão abrirá automaticamente com o Swagger:
- **Swagger UI:** [https://localhost:5001/swagger](https://localhost:5001/swagger)

Se não abrir automaticamente, acesse manualmente:
- HTTP: [http://localhost:5000/swagger](http://localhost:5000/swagger)
- HTTPS: [https://localhost:5001/swagger](https://localhost:5001/swagger)

### 2.5 Executar Benchmarks

1. No **Solution Explorer**, clique com o botão direito em `ConectaAtende.Benchmarks`
2. Selecione **Set as Startup Project**
3. Altere a configuração para **Release**:
   - Na barra de ferramentas, mude de `Debug` para `Release`
4. Pressione `F5` ou `Ctrl+F5`

### 2.6 Debug no Visual Studio

1. Coloque breakpoints clicando na margem esquerda do editor
2. Pressione `F5` para iniciar o debug
3. Use as ferramentas de debug:
   - `F10` - Step Over (próxima linha)
   - `F11` - Step Into (entrar na função)
   - `Shift+F11` - Step Out (sair da função)
   - `F5` - Continue

### 2.7 Explorar o Código

**Solution Explorer:**
- Visualize todos os projetos e arquivos
- Organizado por camadas (Domain, Application, Infrastructure, API)

**IntelliSense:**
- Autocompletar código (`Ctrl+Space`)
- Informações sobre tipos e métodos ao passar o mouse
- Sugestões de correção automática

---

## 🧪 Testando a API

### Usando o Swagger UI

1. Acesse [http://localhost:5000/swagger](http://localhost:5000/swagger)
2. Expanda um endpoint (ex: `POST /api/contacts`)
3. Clique em **Try it out**
4. Preencha os dados (exemplo abaixo)
5. Clique em **Execute**

**Exemplo - Criar Contato:**
```json
{
  "name": "João Silva",
  "phones": ["11987654321", "1133334444"]
}
```

### Usando cURL (Terminal)

**Criar Contato:**
```bash
curl -X POST http://localhost:5000/api/contacts \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"João Silva\",\"phones\":[\"11987654321\"]}"
```

**Listar Contatos:**
```bash
curl http://localhost:5000/api/contacts?page=1&pageSize=10
```

**Buscar por Nome:**
```bash
curl "http://localhost:5000/api/contacts/search?name=João"
```

**Criar Ticket:**
```bash
curl -X POST http://localhost:5000/api/tickets \
  -H "Content-Type: application/json" \
  -d "{\"contactId\":\"SEU-GUID-AQUI\",\"description\":\"Problema com login\",\"priority\":\"High\"}"
```

### Usando Postman ou Insomnia

1. Importe a coleção do Swagger:
   - No Swagger UI, copie a URL do JSON
   - Importe no Postman/Insomnia

2. Ou crie requisições manualmente:
   - **Base URL:** `http://localhost:5000/api`
   - **Method:** POST, GET, PUT, DELETE
   - **Headers:** `Content-Type: application/json`

---

## 🔧 Comandos Úteis

### Build e Restore

```bash
# Restaurar pacotes NuGet
dotnet restore

# Compilar a solution
dotnet build

# Compilar em modo Release
dotnet build -c Release

# Limpar arquivos de build
dotnet clean
```

### Executar Projetos

```bash
# Executar API
cd src/ConectaAtende.API
dotnet run

# Executar Benchmarks
cd src/ConectaAtende.Benchmarks
dotnet run -c Release
```

### Popular Dados de Teste

```bash
# Após iniciar a API, em outro terminal:
curl http://localhost:5000/api/dev/seed?count=100
```

Ou acesse no navegador:
```
http://localhost:5000/api/dev/seed?count=100
```

---

## 🐛 Solução de Problemas

### Erro: "dotnet não é reconhecido"

**Solução:**
1. Verifique se o .NET 8 SDK está instalado
2. Reinicie o terminal/IDE
3. Verifique as variáveis de ambiente PATH

### Erro: "Porta já em uso"

**Solução:**
1. Altere a porta no `launchSettings.json`:
   ```json
   "applicationUrl": "http://localhost:5002;https://localhost:5003"
   ```
2. Ou encerre o processo que está usando a porta:
   ```bash
   # Windows PowerShell
   netstat -ano | findstr :5000
   taskkill /PID <PID> /F
   ```

### Erro: "Package restore failed"

**Solução:**
```bash
# Limpar cache do NuGet
dotnet nuget locals all --clear

# Restaurar novamente
dotnet restore
```

### Swagger não abre

**Solução:**
1. Verifique se está em modo Development
2. Verifique o arquivo `Program.cs` - Swagger deve estar habilitado
3. Tente acessar diretamente: `http://localhost:5000/swagger/index.html`

### Benchmarks não executam

**Solução:**
1. Certifique-se de executar em modo **Release**:
   ```bash
   dotnet run -c Release
   ```
2. No Visual Studio, altere a configuração para Release na barra de ferramentas

---

## 📚 Estrutura do Projeto

```
ConectaAtende API (.NET 8)/
├── src/
│   ├── ConectaAtende.Domain/          # Entidades e interfaces
│   ├── ConectaAtende.Application/      # Serviços e DTOs
│   ├── ConectaAtende.Infrastructure/   # Implementações InMemory
│   ├── ConectaAtende.API/             # Controllers e configuração
│   └── ConectaAtende.Benchmarks/      # Projeto de benchmarks
├── ConectaAtende.sln                   # Solution file
├── README.md                           # Documentação principal
├── PROGRESSO.md                        # Progresso do projeto
└── GUIA_USO.md                         # Este arquivo
```

---

## 🎓 Próximos Passos

1. **Explorar os Endpoints:**
   - Teste todos os endpoints no Swagger
   - Crie contatos, tickets, teste as buscas

2. **Entender a Arquitetura:**
   - Leia o `README.md` para entender a arquitetura
   - Explore as camadas: Domain → Application → Infrastructure → API

3. **Executar Benchmarks:**
   - Execute os benchmarks e analise os resultados
   - Compare a estrutura associativa didática com Dictionary padrão

4. **Modificar e Experimentar:**
   - Adicione novos endpoints
   - Crie novas políticas de triagem
   - Experimente com a estrutura associativa

---

## 📞 Suporte

Se encontrar problemas:

1. Verifique os logs no console
2. Consulte o `README.md` para detalhes técnicos
3. Verifique o `PROGRESSO.md` para ver o que foi implementado
4. Revise a seção "Solução de Problemas" acima

---

## ✅ Checklist Rápido

- [ ] .NET 8 SDK instalado
- [ ] Visual Studio Code ou Visual Studio instalado
- [ ] Extensões instaladas (VS Code)
- [ ] Projeto aberto na IDE
- [ ] Dependências restauradas (`dotnet restore`)
- [ ] API executando (`dotnet run`)
- [ ] Swagger acessível em `http://localhost:5000/swagger`
- [ ] Testou criar um contato
- [ ] Testou criar um ticket

---

**Bom desenvolvimento! 🚀**
