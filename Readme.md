O Universo de Átomos
5 tipos com personalidades: * Luminar (luz), # Umbral (sombra), @ Nexus (hub), ~ Pulsar (onda), . Void (vazio)
3 regras de conexão: proximidade, compatibilidade energética, afinidade de tipo
Matriz de afinidade: Luminar↔Umbral se atraem fortemente, Nexus conecta com todos, Void consome e destrói
O Ciclo de Vida (cada tick)
Spawn → Move → Age → Energy → Connect → Disconnect → Log → Render ASCII → Persist

API Endpoints (Swagger em /swagger)
POST /api/universe — criar universo (Big Bang)
POST /api/universe/{id}/tick — evoluir 1 tick
GET /api/universe/{id}/ascii — ver arte ASCII atual
GET /api/universe/{id}/analytics — estatísticas do universo
GET /api/universe/{id}/history — log de interações
Para rodar
Configure a connection string do MySQL em appsettings.json
Gere a migration: dotnet ef migrations add InitialCreate --project src/ZulAi.Infrastructure --startup-project src/ZulAi.Api
Aplique: dotnet ef database update --project src/ZulAi.Infrastructure --startup-project src/ZulAi.Api
Execute: dotnet run --project src/ZulAi.Api