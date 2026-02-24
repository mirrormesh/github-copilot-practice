@echo off
setlocal

set "GITHUB_MODELS_TOKEN=<YOUR_GITHUB_MODELS_TOKEN>"
set "GITHUB_MODELS_MODEL=gpt-4o-mini"
set "GITHUB_MODELS_ENDPOINT=https://models.inference.ai.azure.com"

echo [1/2] 현재 CMD 세션 환경변수 설정 완료
echo [2/2] 사용자 영구 환경변수(setx) 설정 중...

setx GITHUB_MODELS_TOKEN "%GITHUB_MODELS_TOKEN%" >nul
setx GITHUB_MODELS_MODEL "%GITHUB_MODELS_MODEL%" >nul
setx GITHUB_MODELS_ENDPOINT "%GITHUB_MODELS_ENDPOINT%" >nul

echo 설정 완료. 새 터미널을 열어야 dotnet 실행에 반영됩니다.
echo 확인 명령: echo %%GITHUB_MODELS_TOKEN%%

endlocal
