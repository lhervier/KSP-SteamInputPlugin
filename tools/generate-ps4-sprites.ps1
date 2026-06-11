# Generates the PS4 button sprites (white shapes on transparent background) used by the
# TMP sprite asset of the plugin UI. Shapes are drawn at 512x512 with anti-aliasing, then
# downscaled to the target size for a clean result.
param(
    [string]$OutDir = "$PSScriptRoot\..\SteamInputPlugin\GameData\SteamInputMod\Textures",
    [int]$Size = 72
)

Add-Type -AssemblyName System.Drawing

$big = 512
$penWidth = 44.0

function New-ShapePng {
    param([string]$Path, [scriptblock]$Draw)

    $bmp = New-Object System.Drawing.Bitmap $big, $big, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $pen = New-Object System.Drawing.Pen ([System.Drawing.Color]::White), $penWidth
    $pen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $pen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round

    & $Draw $g $pen

    $pen.Dispose()
    $g.Dispose()

    $small = New-Object System.Drawing.Bitmap $Size, $Size, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g2 = [System.Drawing.Graphics]::FromImage($small)
    $g2.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g2.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g2.DrawImage($bmp, 0, 0, $Size, $Size)
    $g2.Dispose()
    $small.Save($Path, [System.Drawing.Imaging.ImageFormat]::Png)
    $small.Dispose()
    $bmp.Dispose()
    Write-Host "Wrote $Path"
}

# All shapes share the same bounding box so the four buttons have a consistent visual weight.
$m = 90.0          # margin around the shape
$lo = $m
$hi = $big - $m
$c = $big / 2.0

New-ShapePng -Path (Join-Path $OutDir "ps4_circle.png") -Draw {
    param($g, $pen)
    $g.DrawEllipse($pen, $lo, $lo, $hi - $lo, $hi - $lo)
}

New-ShapePng -Path (Join-Path $OutDir "ps4_square.png") -Draw {
    param($g, $pen)
    $g.DrawRectangle($pen, $lo, $lo, $hi - $lo, $hi - $lo)
}

New-ShapePng -Path (Join-Path $OutDir "ps4_triangle.png") -Draw {
    param($g, $pen)
    # Equilateral-looking triangle centered in the box, apex up.
    $points = @(
        (New-Object System.Drawing.PointF $c, $lo),
        (New-Object System.Drawing.PointF $hi, $hi),
        (New-Object System.Drawing.PointF $lo, $hi)
    )
    $g.DrawPolygon($pen, $points)
}

New-ShapePng -Path (Join-Path $OutDir "ps4_cross.png") -Draw {
    param($g, $pen)
    # Slightly inset compared to the outline shapes: a full-box X looks bigger than it is.
    $i = 30.0
    $g.DrawLine($pen, $lo + $i, $lo + $i, $hi - $i, $hi - $i)
    $g.DrawLine($pen, $hi - $i, $lo + $i, $lo + $i, $hi - $i)
}
