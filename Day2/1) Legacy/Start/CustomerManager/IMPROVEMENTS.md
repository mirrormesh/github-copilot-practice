# CustomerManager 개선 이력 (Step-by-Step)

이 문서는 현재 브랜치에서 CustomerManager 프로젝트를 개선한 내용을 단계별로 정리한 기록입니다.

## 1) 초기 상태 점검

- 기존 구조: ASP.NET Core Web API + Controller 기반
- 구현 범위: 건강체크, 고객 조회/검색 중심
- 한계:
  - 고객 생성/수정/삭제 API 부재
  - 전체 목록 조회 API 부재
  - 자동 테스트 부재

---

## 2) 고객 관리 기능 확장 (CRUD + 전체조회)

### 변경 내용

- 고객 전체 목록 조회 추가
- 고객 추가(Create) 추가
- 고객 수정(Update) 추가
- 고객 삭제(Delete) 추가
- 기존 조회(Get by id), 검색(Search) 유지

### 설계/구현 포인트

- 서비스 계층(`CustomerService`)에 CRUD 메서드 확장
- 요청 DTO(`CreateCustomerRequest`, `UpdateCustomerRequest`) 추가
- 요청 간 메모리 데이터 유지를 위해 서비스 수명 `Singleton` 적용

---

## 3) 테스트 체계 도입

### 변경 내용

- `CustomerManager.Tests` 프로젝트 신규 추가
- xUnit 기반 단위 테스트 작성

### 검증 항목

- 초기 고객 목록 조회
- 고객 추가 및 ID 생성
- 고객 수정 반영
- 고객 삭제 반영
- 고객 이름 검색(부분일치/대소문자 무시)

---

## 4) Controller → Minimal API 전환

### 변경 내용

- 컨트롤러 라우트를 `Program.cs` Minimal API 엔드포인트로 이관
- `CustomersController`, `HealthController` 제거

### 효과

- 라우팅/로직 진입점 단순화
- 프로젝트 구조 경량화

---

## 5) Minimal API 간결화

### 변경 내용

- 중복 검증/분기 코드 정리
- 공통 검증 헬퍼 활용
- 응답 분기 표현 단순화

### 효과

- 가독성 향상
- 유지보수 포인트 축소

---

## 6) Microsoft Agent Framework + GitHub Models 연동

### 변경 내용

- 패키지 추가
  - `Microsoft.Agents.AI.OpenAI` (pre-release)
  - `OpenAI`
- `GitHubModelsAgentService` 신규 도입
- `/api/agent/chat` 엔드포인트 추가

### 환경 변수

- `GITHUB_MODELS_TOKEN`
- `GITHUB_MODELS_MODEL`
- `GITHUB_MODELS_ENDPOINT`

---

## 7) GitHub Models 모델 선택 안정화

### 문제

- 토큰/계정에 따라 모델 식별자 문자열 불일치(`unknown_model`) 발생

### 개선 내용

- `/models` 목록 조회 후 사용 가능한 `chat-completion` 모델 자동 해석/선택
- 설정 모델 후보(원문/세그먼트/접두사 변형) 매칭 로직 추가
- 기본 fallback 모델 선택 로직 보강

### 효과

- 환경별 모델 ID 차이로 인한 실패 감소

---

## 8) 응답 가시성 개선 (사용 모델 반환)

### 변경 내용

- `/api/agent/chat` 응답에 실제 사용 모델명을 `model` 필드로 포함

### 결과 예시

```json
{
  "response": "안녕하세요! ...",
  "model": "gpt-4o-mini"
}
```

---

## 9) 운영 편의성 개선

### 9-1) 배치 파일 제공

- `set-github-models-env.bat` 추가
- 환경변수 일괄 설정 + `setx` 영구 저장 지원

### 9-2) README 보강

- 배치 파일 용도/사용법/복구 방법 문서화
- 파일 누락 시 재생성 가능한 템플릿 포함

### 9-3) 배치파일 없이 실행 가능

- 앱 시작 시 토큰 미설정이면 콘솔 입력 프롬프트 제공
- 입력 토큰은 현재 실행 세션에 반영

---

## 10) 검증 이력

- `dotnet build` 성공
- `dotnet test` 성공 (CustomerManager.Tests)
- `/api/agent/chat` 실호출 검증 완료 (200 + 응답/모델 필드 확인)

---

## 참고

- GitHub Models는 GitHub 서비스이지만, 추론 엔드포인트 도메인은 `models.inference.ai.azure.com`을 사용합니다.
- 토큰은 코드/문서에 하드코딩하지 않고 환경변수로 관리하는 것을 권장합니다.
