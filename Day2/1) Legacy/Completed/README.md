# GitHub Copilot 실습: 레거시 .NET 프로젝트 현대화

## 📋 실습 목표
1. 레거시 .NET 프로젝트 실행 복구
2. 기존 기능 파악 및 흐름 분석
3. Minimal API로 변경 및 코드 개선
4. 단위 테스트 작성

---

## 🚀 Step 1: 프로젝트 실행 복구

### 1.1 SDK/런타임 확인
```powershell
dotnet --version  # .NET 8.0 이상 필요
```

### 1.2 프로젝트 복원 및 빌드
```powershell
cd "Day2/1) Legacy/Completed"
dotnet restore
dotnet build
```

### 1.3 애플리케이션 실행
```powershell
dotnet run
```

### 1.4 엔드포인트 테스트

#### Health Check
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Get
```
**기대 결과**: Status가 "Healthy"인 JSON 응답

#### 고객 검색
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/customers/search?name=John" -Method Get
```

#### 고객 ID로 조회
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/customers/1" -Method Get
```

#### Swagger UI
브라우저에서 `http://localhost:5000/swagger` 접속

---

## 📂 Step 2: 레거시 기능 파악

### 2.1 프로젝트 구조
```
CustomerManager/
├── Program.cs              # Minimal API 엔드포인트 정의
├── Services/
│   └── CustomerService.cs  # 비즈니스 로직
└── Models/
    └── DomainModels.cs     # 데이터 모델
```

### 2.2 데이터 흐름 분석

**고객 검색 기능 (/api/customers/search?name={name})**
1. **진입점**: `Program.cs` - `MapGet("/api/customers/search")`
2. **서비스**: `ICustomerService.SearchCustomer(string name)`
3. **로직**: 
   - 입력 검증 (null/whitespace 체크)
   - 인메모리 데이터에서 이름으로 검색 (대소문자 무시)
   - 부분 일치 지원
4. **응답**: Customer 객체 또는 404 NotFound

---

## 🔧 Step 3: 코드 개선 실습 (GitHub Copilot 활용)

### 3.1 입력 검증 개선

**현재 문제점**: 고객 검색 시 빈 문자열 처리가 불완전

**Copilot에게 요청**:
```
// TODO: SearchCustomer 메서드에서 null, 빈 문자열, 공백만 있는 경우를 모두 처리하도록 개선해주세요
```

### 3.2 예외 처리 추가

**Copilot 프롬프트**:
```
// CustomerService에 try-catch 블록을 추가하고, 예외 발생 시 로깅하도록 개선해주세요
```**힌트**: `ILogger<CustomerService>`를 DI로 주입

### 3.3 응답 모델 개선

**Copilot에게 요청**:
```
// API 응답을 일관된 형식으로 만들기 위한 ApiResponse<T> 제네릭 클래스를 만들어주세요
// - Success (bool)
// - Data (T)
// - Message (string)
// - ErrorCode (string, nullable)
```

---

## ✅ Step 4: 단위 테스트 작성 (GitHub Copilot 활용)

### 4.1 테스트 프로젝트 생성

```powershell
dotnet new xunit -n CustomerManager.Tests -o Tests
cd Tests
dotnet add reference ../CustomerManager.csproj
cd ..
dotnet sln add Tests/CustomerManager.Tests.csproj
```

### 4.2 Copilot으로 테스트 작성

**Tests/CustomerServiceTests.cs 파일을 생성하고 Copilot에게 요청**:

```csharp
// CustomerService에 대한 xUnit 테스트를 작성해주세요.
// 다음 시나리오를 포함해주세요:
// 1. GetCustomer - 유효한 ID로 조회 성공
// 2. GetCustomer - 존재하지 않는 ID로 조회 시 null 반환
// 3. SearchCustomer - 유효한 이름으로 검색 성공
// 4. SearchCustomer - 부분 일치 검색 성공
// 5. SearchCustomer - null/빈 문자열 입력 시 null 반환
// 6. GetAllCustomers - 전체 고객 목록 반환
```

### 4.3 테스트 실행

```powershell
dotnet test
```

---

## 🎯 Step 5: 고급 실습 (선택사항)

### 5.1 페이징 기능 추가

**Copilot 프롬프트**:
```
// GetAllCustomers에 페이징 기능을 추가해주세요
// - pageNumber (기본값: 1)
// - pageSize (기본값: 10)
// - 총 페이지 수, 현재 페이지, 전체 항목 수를 포함하는 PagedResponse<T> 반환
```

### 5.2 DTO 패턴 적용

**Copilot에게 요청**:
```
// Customer 엔티티와 CustomerDto를 분리하고, AutoMapper를 사용하여 매핑해주세요
```

### 5.3 Repository 패턴 적용

**Copilot 프롬프트**:
```
// ICustomerRepository 인터페이스와 InMemoryCustomerRepository 구현체를 만들어주세요
// CustomerService가 Repository를 사용하도록 리팩토링해주세요
```

---

## 💡 Copilot 활용 팁

### 효과적인 프롬프트 작성법
1. **구체적으로 요청**: what, how, why를 명확히
2. **맥락 제공**: 기존 코드 스타일, 사용 중인 라이브러리
3. **예시 포함**: 입력/출력 예시 제공
4. **제약 조건 명시**: 성능, 보안, 코딩 규칙

### 유용한 Copilot 기능
- **Inline Suggestions**: 코드 작성 중 Tab으로 자동완성
- **Copilot Chat**: `/explain` - 코드 설명 요청
- **Copilot Chat**: `/fix` - 버그 수정 요청
- **Copilot Chat**: `/tests` - 테스트 코드 생성

---

## 📝 체크리스트

- [x] 애플리케이션 정상 실행 확인 (health endpoint 200 OK)
- [x] Swagger UI에서 모든 API 테스트 완료
- [x] 고객 검색 기능의 데이터 흐름 이해
- [x] 입력 검증 로직 개선 완료
- [x] 단위 테스트 프로젝트 생성 및 테스트 작성
- [x] 모든 테스트 통과 확인
- [x] (선택) 페이징/DTO/Repository 패턴 적용

---

## 🔄 변경 이력

### Minimal API로 전환 완료
- ✅ Controller 기반에서 Minimal API로 변경
- ✅ Swagger 문서화 유지
- ✅ 의존성 주입 패턴 유지
- ✅ 불필요한 Controller 파일 제거

### 개선된 기능
- ✅ 더 명확한 에러 메시지
- ✅ HTTP 상태 코드 개선 (BadRequest, NotFound 적절히 사용)
- ✅ OpenAPI 태그 및 문서화 개선

---

## 🆘 문제 해결

### 빌드 오류 발생 시
```powershell
dotnet clean
Remove-Item -Recurse -Force obj, bin
dotnet restore
dotnet build
```

### 포트 충돌 시
`appsettings.json`에서 포트 변경:
```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5001"
      }
    }
  }
}
```

### Copilot 제안이 나오지 않을 때
1. VS Code에서 Copilot 로그인 상태 확인
2. 파일 확장자가 `.cs`인지 확인
3. 주석으로 명확한 의도 작성 후 새 줄 추가

---

## 🎓 다음 단계

이 실습을 완료했다면:
1. **Step 2**: Azure Functions로 마이그레이션
2. **Step 3**: Entity Framework Core 추가
3. **Step 4**: Azure App Service 배포
4. **Step 5**: CI/CD 파이프라인 구성
