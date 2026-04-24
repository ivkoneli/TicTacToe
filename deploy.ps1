# deploy.ps1 — Deploy WebGL build to gh-pages branch
# Run from project root after building to the WebGL/ folder in Unity.
$ErrorActionPreference = "Stop"

$projectRoot  = Split-Path -Parent $MyInvocation.MyCommand.Path
$webglPath    = Join-Path $projectRoot "WebGL"
$worktreePath = Join-Path $projectRoot ".gh-pages-deploy"

if (-not (Test-Path $webglPath)) {
    Write-Host "ERROR: WebGL/ folder not found." -ForegroundColor Red
    Write-Host "Build first in Unity: File -> Build Settings -> Build -> select WebGL/ folder inside the project."
    exit 1
}

# Remove leftover worktree if previous run crashed
if (Test-Path $worktreePath) {
    git worktree remove $worktreePath --force 2>$null
    Remove-Item $worktreePath -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "Checking out gh-pages branch as worktree..." -ForegroundColor Cyan
git worktree add $worktreePath gh-pages

Write-Host "Clearing old build files..." -ForegroundColor Cyan
Get-ChildItem $worktreePath -Exclude ".git" | Remove-Item -Recurse -Force

Write-Host "Copying new WebGL build..." -ForegroundColor Cyan
Copy-Item "$webglPath\*" $worktreePath -Recurse -Force

# Inject canvas resize script so Unity's Screen.width/height match the browser window.
# This makes LayoutSwitcher correctly detect portrait vs landscape on any device.
$indexPath = Join-Path $worktreePath "index.html"
if (Test-Path $indexPath) {
    $resizeScript = @'
<script>
  function resizeUnityCanvas() {
    var canvas = document.querySelector("canvas");
    if (!canvas) return;
    var w = window.innerWidth;
    var h = window.innerHeight;
    canvas.style.width  = w + "px";
    canvas.style.height = h + "px";
    if (typeof unityInstance !== "undefined" && unityInstance) {
      unityInstance.Module.setCanvasSize(w, h);
    }
  }
  window.addEventListener("load",   resizeUnityCanvas);
  window.addEventListener("resize", resizeUnityCanvas);
</script>
'@
    $html = Get-Content $indexPath -Raw
    $html = $html -replace '</body>', "$resizeScript`n</body>"
    Set-Content $indexPath $html -Encoding UTF8
    Write-Host "Canvas resize script injected into index.html" -ForegroundColor Cyan
} else {
    Write-Host "WARNING: index.html not found, skipping resize inject." -ForegroundColor Yellow
}

Push-Location $worktreePath
git add .
$date = Get-Date -Format "yyyy-MM-dd HH:mm"
git commit -m "Deploy WebGL build $date"
git push origin gh-pages
Pop-Location

git worktree remove $worktreePath --force
Write-Host ""
Write-Host "Deployed! -> https://ivkoneli.github.io/TicTacToe/" -ForegroundColor Green
