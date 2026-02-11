# task_20260209
Employee management API supporting CSV and JSON data import

# 📢 프로젝트 요약 및 안내
(커밋 메시지 인코딩 문제로 인해 상세 구현 내용을 여기에 기재합니다.)
- **구현 기능**: 사원 페이징 조회, 이름 검색, CSV 임포트 완료
- **테스트**: xUnit 기반 성공/실패 케이스 테스트 코드 작성
- **문서화**: .NET 9 OpenAPI/Swagger UI 적용
  
---

## Employee Management API

직원의 기본 연락 정보를 관리하기 위한 REST API입니다.  
직원 정보는 CSV 또는 JSON 파일을 통해 추가할 수 있습니다.

A REST API for managing employee basic contact information.  
Employee data can be imported using CSV or JSON files.

---

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- MSSQL

---

## Features

### Korean
- 직원 기본 연락 정보 조회
- CSV 파일을 통한 직원 정보 추가
- JSON 파일을 통한 직원 정보 추가

### English
- Retrieve employee contact information
- Import employees via CSV file
- Import employees via JSON file

---

## Employee Data Format

### JSON (Korean Example)
```json
[
  {
    "name": "홍길동",
    "email": "hong@test.com",
    "tel": "010-1234-5678",
    "joined": "2012-01-05"
  }
]
```

### JSON (English Example)
```json
[
  {
    "name": "John Doe",
    "email": "john@test.com",
    "tel": "010-1234-5678",
    "joined": "2012-01-05"
  }
]
```

### CSV (Korean Example)
```csv
name,email,tel,joined
홍길동,hong@test.com,010-1234-5678,2012-01-05
```

### CSV (English Example)
```csv
name,email,tel,joined
John Doe,john@test.com,010-1234-5678,2012-01-05
```



