# Sistema de Áudio - Configuração

## Como configurar o efeito sonoro quando a fome é incrementada

### Opção 1: Usando AudioManager Global (Recomendado)

1. **Criar o AudioManager:**
   - Crie um GameObject vazio na cena
   - Nomeie como "AudioManager"
   - Adicione o componente `AudioManager.cs`
   - Adicione um componente `AudioSource`

2. **Configurar o AudioManager:**
   - No Inspector do AudioManager, arraste o arquivo de áudio desejado (ex: `Gulp.mp4`) para o campo "Eat Sound"
   - Ajuste o volume no campo "Eat Sound Volume" (0.0 a 1.0)

3. **Configurar o FoodCollect:**
   - No GameObject que tem o script `FoodCollect.cs`
   - Marque a caixa "Use Global Audio Manager"
   - O som será tocado automaticamente quando a fome for incrementada

### Opção 2: Usando AudioSource Local

1. **Configurar o FoodCollect:**
   - No GameObject que tem o script `FoodCollect.cs`
   - Desmarque a caixa "Use Global Audio Manager"
   - Arraste um AudioSource para o campo "Audio Source" (ou deixe vazio para criar automaticamente)
   - Arraste o arquivo de áudio desejado para o campo "Eat Sound"

### Arquivos de Áudio Disponíveis

Na pasta `Assets/Audio/` você tem os seguintes arquivos disponíveis:
- `Gulp.mp4` - Som de engolir (recomendado para comer)
- `Pa.mp4` - Som curto
- `Pu.mp4` - Som curto
- `SLWEEUWW.mp4` - Som longo
- `Hangueee.mp4` - Som longo
- `Bythewaybaba.mp4` - Som longo
- `Wabubaubaa.mp4` - Som longo
- `Wibibawa.mp4` - Som médio
- `Bliwhara.mp4` - Som longo
- `Wbababa Ibawa.mp4` - Som longo

### Como Funciona

O sistema toca o som automaticamente quando:
1. O personagem colide com um objeto com tag "BigMushroom"
2. Está segurando um objeto com tag "Food"
3. O `HungerManager.currentHunger` é incrementado em +10

### Dicas

- Use o `Gulp.mp4` para o som de comer, pois é o mais apropriado
- Ajuste o volume para não ficar muito alto
- O AudioManager global é mais eficiente se você tiver vários objetos que precisam tocar sons
- Se você quiser adicionar mais sons, pode expandir o AudioManager com novos métodos 