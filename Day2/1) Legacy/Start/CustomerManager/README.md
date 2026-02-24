# CustomerManager 실행 안내

## `set-github-models-env.bat` 파일 설명

`set-github-models-env.bat` 파일은 GitHub Models 연동에 필요한 환경변수를 한 번에 설정합니다.

- `GITHUB_MODELS_TOKEN`
- `GITHUB_MODELS_MODEL`
- `GITHUB_MODELS_ENDPOINT`

실행 시:
1. 현재 CMD 세션에 변수 설정
2. `setx`로 사용자 영구 환경변수 저장

> 저장 후에는 **새 터미널**을 열어야 반영됩니다.

---

## 파일이 없어서 실행이 안 될 때

다음 경로에 파일이 있어야 합니다.

- `set-github-models-env.bat`

없다면 동일 이름으로 다시 만들고 아래 내용을 넣으세요.

```bat
@echo off
setlocal

set "GITHUB_MODELS_TOKEN=<YOUR_TOKEN>"
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
```

---

## 배치파일 없이 실행하는 방법

이제 프로그램은 시작 시 토큰이 없으면 콘솔에서 직접 입력받습니다.

- 실행: `dotnet run --project "CustomerManager.csproj"`
- 토큰 미설정 시 프롬프트 표시:
  - `GITHUB_MODELS_TOKEN이 없습니다. 토큰을 입력하세요(없으면 Enter):`

입력한 토큰은 **현재 실행 세션에만 적용**됩니다.
(영구 저장이 필요하면 배치파일 사용)

---

## 기본 모델/엔드포인트

- 기본 모델: `gpt-4o-mini`
- 기본 엔드포인트: `https://models.inference.ai.azure.com`

`/api/agent/chat` 응답에는 실제 사용된 모델명이 `model` 필드로 포함됩니다.
