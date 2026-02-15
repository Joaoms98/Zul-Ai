# Zul-Ai

Simulador de mini universo. Atomos nascem aleatoriamente, se movem, formam conexoes entre si por regras fisicas, vivem e morrem — tudo renderizado em arte ASCII em tempo real.

HOSPEDADO AQUI POR ENQUANTO!!!
http://3.19.142.106/


Backend em .NET 8 Web API com Entity Framework Core + MySQL. Frontend em React com estetica de terminal.

```
+------------------------------------------------------------------------------+
|                                                                              |
|           *-----------@                                                      |
|          :             \                                                     |
|         #....~          @========*                                           |
|              |                                                               |
|              :                                                               |
|              #                  ~                                            |
|                                 :                                            |
|    *                            .                                            |
|                                                                              |
+------------------------------------------------------------------------------+
 Atoms: 8 | Connections: 6 | Energy: 412.7
```

---

## Arquitetura

```
Zul-Ai/
├── src/
│   ├── ZulAi.Domain/            Entidades, enums, interfaces de repositorio
│   ├── ZulAi.Application/       Servicos, regras de conexao, renderizador ASCII
│   ├── ZulAi.Infrastructure/    EF Core, MySQL, repositorios
│   ├── ZulAi.Api/               Web API, controllers, Swagger
│   └── zulai-frontend/          React + Vite + TypeScript
└── Zul-Ai.sln
```

| Camada | Responsabilidade | Dependencias externas |
|--------|------------------|-----------------------|
| Domain | Entidades puras e contratos | Nenhuma |
| Application | Logica de simulacao e regras | Domain |
| Infrastructure | Persistencia MySQL via Pomelo EF Core | Domain |
| Api | Endpoints REST, DI, CORS | Application, Infrastructure |
| Frontend | Interface terminal em React | API via proxy |

---

## Os Atomos

Existem 5 tipos fundamentais de atomos. Cada tipo tem personalidade, comportamento e papel no universo.

### Tipos

| Tipo | Simbolo | Energia Inicial | Velocidade | Vida Maxima | Comportamento |
|------|---------|-----------------|------------|-------------|---------------|
| **Luminar** | `*` | 60–100 | 1.5 u/tick | 150 ticks | Irradia luz. Doa energia a atomos proximos num raio de 10 unidades. Atrai Umbral. Veloz e brilhante. |
| **Umbral** | `#` | 30–70 | 0.8 u/tick | 200 ticks | Sombra. Absorve energia. Atraido fortemente por Luminar — opostos se atraem. Resistente. |
| **Nexus** | `@` | 40–80 | 0.5 u/tick | 250 ticks | Hub social. Conecta facilmente com todos os tipos. Lento mas longevo. O tecido conectivo do universo. |
| **Pulsar** | `~` | 50–95 | 2.0 u/tick | 80 ticks | Onda instavel. Energia oscila com `sin(idade * 0.3) * 2.0`. O mais rapido, mas vida curta. Explode em atividade. |
| **Void** | `.` | 10–40 | 0.3 u/tick | 300 ticks | Vazio. Drena energia de vizinhos num raio de 8 unidades e absorve 50% do que drena. Destroi conexoes. Quase invisivel, quase eterno. |

### Propriedades de cada atomo

- **Posicao (X, Y):** coordenadas no grid (padrao 80x40)
- **Energia:** 0.0 a 100.0 — chega a zero, o atomo morre
- **Idade:** incrementa a cada tick — ultrapassa o limite do tipo, morre
- **Vivo/Morto:** atomos mortos permanecem no banco mas nao participam da simulacao

---

## Regras de Conexao

Atomos se conectam quando a soma ponderada de 3 regras ultrapassa o limiar de **0.55**.

### Regra 1: Proximidade (peso 0.4)

Distancia euclidiana entre dois atomos. Raio maximo de deteccao: **15 unidades**.

```
score = 1.0 - (distancia / 15.0)
```

Se a distancia for >= 15, score = 0. Atomos precisam estar perto para se conectar.

### Regra 2: Compatibilidade Energetica (peso 0.3)

Atomos com diferenca de energia moderada (~30 unidades) sao mais compativeis. Nem iguais demais, nem opostos demais.

```
diferenca = |energiaA - energiaB|
score = 1.0 - |diferenca - 30.0| / 50.0
```

Resultado fixado entre 0 e 1. O ponto ideal e uma diferenca de 30 — como cargas complementares.

### Regra 3: Afinidade de Tipo (peso 0.3)

Matriz de afinidade que define como cada par de tipos interage:

```
             Luminar   Umbral   Nexus   Pulsar   Void
Luminar        0.2      0.8      0.6     0.5     0.1
Umbral         0.8      0.1      0.5     0.3     0.7
Nexus          0.6      0.5      0.4     0.7     0.3
Pulsar         0.5      0.3      0.7     0.2     0.4
Void           0.1      0.7      0.3     0.4     0.0
```

Destaques:
- **Luminar ↔ Umbral = 0.8** — luz e sombra se atraem intensamente
- **Nexus ↔ Pulsar = 0.7** — ondas propagam atraves de hubs
- **Umbral ↔ Void = 0.7** — escuridao e vazio se entendem
- **Void ↔ Void = 0.0** — o vazio nao se conecta consigo mesmo
- **Nexus** tem afinidade >= 0.3 com todos — e o conector universal

### Formacao de Conexao

```
scoreFinal = (proximidade * 0.4) + (energia * 0.3) + (afinidade * 0.3)

Se scoreFinal >= 0.55 → conexao formada com forca = scoreFinal
```

### Quebra de Conexao

Conexoes se rompem quando:
- Um dos atomos morre (energia = 0 ou idade maxima)
- Atomos se afastam mais de **20 unidades**
- Void drena a forca da conexao em **0.1 por tick** — abaixo de 0.2, quebra

---

## Ciclo do Tick

Cada tick executa 9 fases em sequencia:

```
1. SPAWN      Cria 1-3 atomos aleatorios (chance inversamente proporcional a populacao)
2. MOVE       Cada atomo se move aleatoriamente pela velocidade do seu tipo
3. AGE        Incrementa idade. Mata atomos velhos ou sem energia
4. ENERGY     Luminar doa energia. Void drena. Pulsar oscila. Todos perdem 0.5/tick
5. CONNECT    Avalia pares de atomos vivos pelas 3 regras. Forma novas conexoes
6. DISCONNECT Quebra conexoes de atomos mortos, distantes ou drenados por Void
7. LOG        Registra nascimentos, mortes, conexoes e desconexoes no banco
8. RENDER     Gera arte ASCII do estado atual no grid
9. PERSIST    Salva tudo no MySQL em uma transacao
```

---

## Arte ASCII

O renderizador converte o estado do universo em um grid de caracteres.

### Camadas de renderizacao

1. **Fundo:** espacos vazios
2. **Bordas:** `+` nos cantos, `-` horizontal, `|` vertical
3. **Conexoes:** linhas entre atomos usando algoritmo de Bresenham
4. **Atomos:** simbolo do tipo desenhado por cima

### Caracteres de conexao por forca

| Forca | Caractere | Significado |
|-------|-----------|-------------|
| > 0.8 | `=` | Conexao forte |
| > 0.5 | `-` | Conexao media |
| > 0.2 | `:` | Conexao fraca |
| <= 0.2 | `.` | Conexao minima |

---

## API Endpoints

Base URL: `http://localhost:5187/api`

### Universo

| Metodo | Rota | Descricao |
|--------|------|-----------|
| POST | `/universe` | Criar universo (Big Bang). Body: `{ width, height, initialAtoms }` |
| GET | `/universe/{id}` | Estado atual com atomos vivos |
| POST | `/universe/{id}/tick` | Avancar 1 tick |
| POST | `/universe/{id}/tick/{n}` | Avancar N ticks (max 100) |
| GET | `/universe/{id}/analytics` | Estatisticas: nascimentos, mortes, energia media, populacao por tipo |

### Arte ASCII

| Metodo | Rota | Descricao |
|--------|------|-----------|
| GET | `/universe/{id}/ascii` | ASCII art atual (Content-Type: text/plain) |
| GET | `/universe/{id}/ascii/json` | ASCII art atual como JSON com metadados |
| GET | `/universe/{id}/ascii/history` | Historico de outputs paginado |

### Atomos

| Metodo | Rota | Descricao |
|--------|------|-----------|
| GET | `/universe/{id}/atoms` | Listar atomos vivos. Query: `?type=Luminar` |
| GET | `/universe/{id}/atoms/{atomId}` | Detalhes de um atomo |
| GET | `/universe/{id}/atoms/{atomId}/connections` | Conexoes de um atomo |

### Historico

| Metodo | Rota | Descricao |
|--------|------|-----------|
| GET | `/universe/{id}/history` | Log de interacoes paginado. Query: `?type=Born&skip=0&take=50` |

Swagger disponivel em `http://localhost:5187/swagger` em ambiente de desenvolvimento.

---

## Banco de Dados

MySQL via Pomelo.EntityFrameworkCore.MySql. 5 tabelas:

- **UniverseStates** — grid, tick atual, status ativo
- **Atoms** — tipo, posicao, energia, idade, simbolo, vivo/morto
- **AtomConnections** — par de atomos, forca, tick de formacao, ativo/inativo
- **Interactions** — log de eventos (nasceu, morreu, conectou, desconectou)
- **GeneratedOutputs** — arte ASCII por tick com metadados

---

## Como Rodar

### Pre-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [MySQL 8.0](https://dev.mysql.com/downloads/)

### 1. Configurar banco de dados

Edite a connection string em `src/ZulAi.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ZulAiDb": "Server=localhost;Port=3306;Database=zulai;User=root;Password=SUA_SENHA;"
  }
}
```

Crie o banco no MySQL:

```sql
CREATE DATABASE zulai;
```

### 2. Aplicar migrations

```bash
dotnet ef migrations add InitialCreate \
  --project src/ZulAi.Infrastructure \
  --startup-project src/ZulAi.Api

dotnet ef database update \
  --project src/ZulAi.Infrastructure \
  --startup-project src/ZulAi.Api
```

### 3. Rodar o backend

```bash
dotnet run --project src/ZulAi.Api
```

O backend inicia em `http://localhost:5187`. Swagger em `http://localhost:5187/swagger`.

### 4. Rodar o frontend

```bash
cd src/zulai-frontend
npm install
npm run dev
```

O frontend inicia em `http://localhost:5173` com proxy automatico para o backend.

### 5. Usar

1. Abra `http://localhost:5173`
2. Clique **BIG BANG** para criar o universo
3. Clique **TICK** para evoluir ou **AUTO PLAY** para simulacao continua
4. Observe atomos nascendo, conectando e morrendo na arte ASCII
5. Clique **ANALYTICS** para ver estatisticas de populacao

---

## Stack

| Componente | Tecnologia |
|------------|------------|
| Backend | .NET 8, ASP.NET Core Web API |
| ORM | Entity Framework Core 8.0.2 |
| Banco | MySQL 8.0 via Pomelo |
| Frontend | React 19, TypeScript 5.9, Vite 7 |
| Estilo | CSS puro com estetica de terminal |
