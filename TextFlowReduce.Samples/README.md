# TextFlowReduce.Samples

Ferramenta de análise em lote de respostas usando arquivos CSV.

## 🚀 Início Rápido

### Executar
```bash
dotnet run
```

### Menu
```
=== TextFlowReduce - Análise de Respostas ===

Opções:
1 - Criar arquivo CSV de exemplo
2 - Analisar arquivo CSV existente
```

## 📋 Formato do Arquivo CSV

```csv
Nome do Estudante,01,02,03,04,05,06,07,08,09,10
João Silva,"Resposta questão 01","Resposta questão 02",...
Maria Santos,"Resposta questão 01","Resposta questão 02",...
```

**Regras:**
- Linha 1: Cabeçalhos (Nome do Estudante | 01-10)
- Coluna 1: Nome do estudante
- Colunas 2-11: Respostas para cada questão
- Use aspas duplas se a resposta contiver vírgulas
- Células vazias são permitidas

## 🎯 Questões Disponíveis

1. **Programação:** Classe em POO, Herança, Polimorfismo, Encapsulamento, Recursividade
2. **Eng. Software:** Acoplamento, Coesão
3. **Outras:** Protocolo HTTP, Chave Primária, Memória RAM

## 📈 Sistema de Pontuação

- **40%** - Palavras-chave obrigatórias
- **40%** - Frases obrigatórias
- **20%** - Palavras-chave opcionais (bônus)
- **Aprovação:** Score ≥ 70 pontos

## 📊 Relatórios Gerados

- Resumo por estudante (média, status)
- Análise detalhada (score por questão)
- Estatísticas gerais (média da turma, taxa de aprovação)
- Desempenho por questão
- Identificação de pontos fracos

## 📤 Exportação

Após a análise, você pode exportar os resultados para arquivo `.txt` na Área de Trabalho.

## 🔧 Requisitos

- .NET 8.0
- Nenhuma dependência externa
