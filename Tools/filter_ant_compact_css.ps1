$src = 'D:\Publics\antcss\ant-design-blazor.compact.css'
$dst = 'D:\Publics\Known\Known\wwwroot\css\ant-compact.css'
$keep = @(
    'font-size',
    'line-height',
    'letter-spacing',
    'margin',
    'margin-top',
    'margin-right',
    'margin-bottom',
    'margin-left',
    'padding',
    'padding-top',
    'padding-right',
    'padding-bottom',
    'padding-left',
    'width',
    'min-width',
    'max-width',
    'height',
    'min-height',
    'max-height',
    'top',
    'right',
    'bottom',
    'left',
    'inset',
    'inset-block',
    'inset-block-start',
    'inset-block-end',
    'inset-inline',
    'inset-inline-start',
    'inset-inline-end',
    'border-width',
    'border-top-width',
    'border-right-width',
    'border-bottom-width',
    'border-left-width',
    'border-radius',
    'border-top-left-radius',
    'border-top-right-radius',
    'border-bottom-right-radius',
    'border-bottom-left-radius',
    'gap',
    'row-gap',
    'column-gap',
    'flex-basis',
    'grid-template-columns',
    'grid-template-rows',
    'grid-auto-columns',
    'grid-auto-rows',
    'column-width',
    'tab-size',
    'outline-offset'
)

function Remove-CssComments([string]$text) {
    return [regex]::Replace($text, '/\*.*?\*/', '', [System.Text.RegularExpressions.RegexOptions]::Singleline)
}

function Split-Statements([string]$block) {
    $parts = New-Object System.Collections.Generic.List[string]
    $buffer = New-Object System.Text.StringBuilder
    $parenDepth = 0
    $quote = $null
    $escape = $false

    foreach ($ch in $block.ToCharArray()) {
        if ($quote) {
            [void]$buffer.Append($ch)
            if ($escape) {
                $escape = $false
            }
            elseif ($ch -eq '\\') {
                $escape = $true
            }
            elseif ($ch -eq $quote) {
                $quote = $null
            }
            continue
        }

        if ($ch -eq '"' -or $ch -eq "'") {
            $quote = $ch
            [void]$buffer.Append($ch)
        }
        elseif ($ch -eq '(') {
            $parenDepth++
            [void]$buffer.Append($ch)
        }
        elseif ($ch -eq ')') {
            if ($parenDepth -gt 0) { $parenDepth-- }
            [void]$buffer.Append($ch)
        }
        elseif ($ch -eq ';' -and $parenDepth -eq 0) {
            $part = $buffer.ToString().Trim()
            if ($part) { $parts.Add($part) }
            $buffer.Clear() | Out-Null
        }
        else {
            [void]$buffer.Append($ch)
        }
    }

    $tail = $buffer.ToString().Trim()
    if ($tail) { $parts.Add($tail) }
    return $parts
}

function Find-MatchingBrace([string]$text, [int]$startIndex) {
    $depth = 1
    $quote = $null
    $escape = $false

    for ($i = $startIndex + 1; $i -lt $text.Length; $i++) {
        $ch = $text[$i]
        if ($quote) {
            if ($escape) {
                $escape = $false
            }
            elseif ($ch -eq '\\') {
                $escape = $true
            }
            elseif ($ch -eq $quote) {
                $quote = $null
            }
            continue
        }

        if ($ch -eq '"' -or $ch -eq "'") {
            $quote = $ch
        }
        elseif ($ch -eq '{') {
            $depth++
        }
        elseif ($ch -eq '}') {
            $depth--
            if ($depth -eq 0) {
                return $i
            }
        }
    }

    return $text.Length - 1
}

function Normalize-Rule([string]$header, [System.Collections.Generic.List[string]]$declarations) {
    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add(($header.Trim() + ' {'))
    foreach ($declaration in $declarations) {
        $lines.Add(('  ' + $declaration.Trim() + ';'))
    }
    $lines.Add('}')
    return ($lines -join [Environment]::NewLine)
}

function Process-Css([string]$text, [string[]]$keepProperties) {
    $result = New-Object System.Collections.Generic.List[string]
    $i = 0

    while ($i -lt $text.Length) {
        while ($i -lt $text.Length -and [char]::IsWhiteSpace($text[$i])) {
            $i++
        }
        if ($i -ge $text.Length) { break }

        $braceIndex = $text.IndexOf('{', $i)
        if ($braceIndex -lt 0) { break }

        $header = $text.Substring($i, $braceIndex - $i).Trim()
        $endIndex = Find-MatchingBrace $text $braceIndex
        $inner = $text.Substring($braceIndex + 1, $endIndex - $braceIndex - 1)

        if ($header.StartsWith('@')) {
            $nested = Process-Css $inner $keepProperties
            if (-not [string]::IsNullOrWhiteSpace($nested)) {
                $result.Add(($header + ' {' + [Environment]::NewLine + $nested + [Environment]::NewLine + '}'))
            }
        }
        else {
            $declarations = New-Object System.Collections.Generic.List[string]
            foreach ($statement in (Split-Statements $inner)) {
                if ($statement -notmatch ':') { continue }
                $name = $statement.Split(':', 2)[0].Trim().ToLowerInvariant()
                if ($keepProperties -contains $name) {
                    $declarations.Add($statement)
                }
            }
            if ($declarations.Count -gt 0) {
                $result.Add((Normalize-Rule $header $declarations))
            }
        }

        $i = $endIndex + 1
    }

    return ($result -join ([Environment]::NewLine + [Environment]::NewLine))
}

$text = Get-Content -Path $src -Raw -Encoding UTF8
$text = Remove-CssComments $text
$result = (Process-Css $text $keep).Trim() + [Environment]::NewLine
Set-Content -Path $dst -Value $result -Encoding UTF8
Write-Output ('Written ' + $dst + ' with ' + ($result -split "`r?`n").Count + ' lines.')
