from pathlib import Path
import re

SRC = Path(r"D:\Publics\antcss\ant-design-blazor.dark.css")
DST = Path(r"D:\Publics\Known\Known\wwwroot\css\ant-dark.css")

KEEP_PROPERTIES = {
    "color",
    "background",
    "background-color",
    "border",
    "border-top",
    "border-right",
    "border-bottom",
    "border-left",
    "border-color",
    "border-top-color",
    "border-right-color",
    "border-bottom-color",
    "border-left-color",
}


def strip_comments(text: str) -> str:
    return re.sub(r"/\*.*?\*/", "", text, flags=re.S)


def split_statements(block: str) -> list[str]:
    parts: list[str] = []
    buffer: list[str] = []
    paren_depth = 0
    quote = ""
    escape = False

    for ch in block:
        if quote:
            buffer.append(ch)
            if escape:
                escape = False
            elif ch == "\\":
                escape = True
            elif ch == quote:
                quote = ""
            continue

        if ch in ('"', "'"):
            quote = ch
            buffer.append(ch)
        elif ch == "(":
            paren_depth += 1
            buffer.append(ch)
        elif ch == ")":
            paren_depth = max(0, paren_depth - 1)
            buffer.append(ch)
        elif ch == ";" and paren_depth == 0:
            part = "".join(buffer).strip()
            if part:
                parts.append(part)
            buffer = []
        else:
            buffer.append(ch)

    tail = "".join(buffer).strip()
    if tail:
        parts.append(tail)
    return parts


def find_matching_brace(text: str, start_index: int) -> int:
    depth = 1
    quote = ""
    escape = False
    i = start_index + 1

    while i < len(text):
        ch = text[i]
        if quote:
            if escape:
                escape = False
            elif ch == "\\":
                escape = True
            elif ch == quote:
                quote = ""
        else:
            if ch in ('"', "'"):
                quote = ch
            elif ch == "{":
                depth += 1
            elif ch == "}":
                depth -= 1
                if depth == 0:
                    return i
        i += 1

    return len(text) - 1


def keep_declaration(statement: str) -> bool:
    if ":" not in statement:
        return False
    prop = statement.split(":", 1)[0].strip().lower()
    return prop in KEEP_PROPERTIES


def normalize_rule(header: str, declarations: list[str]) -> str:
    body = "\n  ".join(f"{declaration.strip()};" for declaration in declarations)
    return f"{header} {{\n  {body}\n}}"


def process_css(text: str) -> str:
    result: list[str] = []
    i = 0
    length = len(text)

    while i < length:
        while i < length and text[i].isspace():
            i += 1
        if i >= length:
            break

        brace_index = text.find("{", i)
        if brace_index < 0:
            break

        header = text[i:brace_index].strip()
        end_index = find_matching_brace(text, brace_index)
        inner = text[brace_index + 1:end_index]

        if header.startswith("@"):
            nested = process_css(inner)
            if nested.strip():
                result.append(f"{header} {{\n{nested}\n}}")
        else:
            declarations = [statement for statement in split_statements(inner) if keep_declaration(statement)]
            if declarations:
                result.append(normalize_rule(header, declarations))

        i = end_index + 1

    return "\n\n".join(result)


source = strip_comments(SRC.read_text(encoding="utf-8-sig"))
filtered = process_css(source).strip() + "\n"
DST.write_text(filtered, encoding="utf-8")
print(f"Written {DST} with {len(filtered.splitlines())} lines.")
