```markdown
# TOOLWEAVER // AI_CONTEXT
# PURPOSE: Local AI agent with strict JSON tool-calling (.NET 8)

## 📌 PROJECT_SCOPE
Build a native C# console application that:
- Calls LLM APIs (Groq/OpenRouter) via HTTP
- Parses structured JSON tool calls from responses
- Executes whitelisted local functions safely
- Returns results to the model for multi-step reasoning
- Uses zero external AI frameworks

## 🏗️ RUNTIME_ARCHITECTURE (REFERENCE ONLY - DO NOT SIMULATE)
The compiled application will implement:
1. ApiClient → HTTP POST to /chat/completions (stream: true)
2. ResponseParser → Extracts `<tool_call>{...}</tool_call>` blocks, validates JSON Schema
3. ToolRegistry → Maps tool names to C# delegates
4. SafeExecutor → Runs functions with path sandboxing, 10s timeout, atomic writes
5. SessionManager → Manages history, enforces max_iterations=5 at runtime

⚠️ YOU ARE NOT RUNNING THIS LOOP. YOU ARE ONLY GENERATING C# CODE FOR IT.

## ⚠️ STRICT_AI_RULES
- Generate ONLY complete, compilable C# 8+ code
- Use ONLY `System.*` namespaces (no LangChain, AutoGen, SemanticKernel, Microsoft.Extensions.AI)
- Always include: input validation, try/catch, timeout guards, logging stubs
- File operations: `Path.GetFullPath()`, sandbox check, `tmp → rename` pattern
- Tool JSON shape expected by runtime: `{"tool":"name","params":{...}}`
- NEVER output placeholders (`// ...`, `// implement later`, `/* code */`)
- If requirements are unclear: ask ONE direct question, then STOP

## 📝 RESPONSE_FORMAT (MANDATORY)
When generating, output EXACTLY:
```csharp
// [FileName.cs]
[complete code here]