# Capstone Review Slot Management System

## Overview

Capstone Review Slot Management System là hệ thống quản lý và tự động hóa quy trình đăng ký, phân công, và tổng hợp lịch review cho môn Capstone Project.

Hệ thống thay thế hoàn toàn quy trình quản lý thủ công bằng Excel hiện tại, nơi giảng viên và sinh viên nhập liệu thủ công vào nhiều file riêng biệt, sau đó Trưởng bộ môn phải tổng hợp và phân công thủ công để tạo timeline review cuối cùng.
<img width="782" height="352" alt="PRN232_3W-System Context Diagram" src="https://github.com/user-attachments/assets/2e3a1f77-12b7-4106-8862-465e192eafc8" />

Mục tiêu của dự án là xây dựng một nền tảng **REST API Microservices bằng .NET 8** giúp:

* Tự động hóa quy trình đăng ký lịch review
* Hạn chế sai sót trong tổng hợp dữ liệu thủ công
* Đảm bảo tuân thủ business rules khi phân công reviewer
* Hỗ trợ mở rộng / deploy Docker / scale microservices trong tương lai

---

## Business Problem

### Quy trình hiện tại (Manual Excel Workflow)

Hiện tại quy trình review Capstone được thực hiện thủ công qua nhiều file Excel:

1. Manager import danh sách đề tài / nhóm sinh viên / giảng viên hướng dẫn
2. Giảng viên tự điền slot rảnh vào file Excel
3. Sinh viên xem slot trống và đăng ký vào file Excel khác
4. Manager tổng hợp toàn bộ dữ liệu
5. Manager tự phân công 2 giảng viên review cho từng nhóm
6. Xuất timeline review cuối cùng cho Review1 / Review2 / Review3

### Vấn đề của quy trình hiện tại

* Dữ liệu nhập tay dễ sai sót
* Tốn nhiều thời gian tổng hợp
* Khó validate business rules
* Khó cân bằng workload giữa giảng viên
* Không realtime
* Không scalable cho số lượng lớn nhóm/sinh viên

---

## Project Objective

Xây dựng hệ thống quản lý review slot cho Capstone Project nhằm:

* Số hóa toàn bộ quy trình review scheduling
* Tự động validate business rules
* Tối ưu workload phân công giảng viên
* Cung cấp API-first architecture cho Web / Mobile / Future Integration
* Hỗ trợ Docker deployment / Cloud-native architecture

---

## Core Workflow

### Step 0 — Manager Setup Review Session

Manager thực hiện:

* Import danh sách đề tài / nhóm / giảng viên từ file Excel
* Tạo đợt review:

  * Review1
  * Review2
  * Review3
* Cấu hình:

  * Ngày review (Mon → Fri)
  * Slots per day (1 → 6)

---

### Step 1 — Lecturer Register Available Slots

Giảng viên đăng ký các slot rảnh của mình theo từng ngày trong tuần.

Ví dụ:

* Monday: Slot 1, 2, 3
* Wednesday: Slot 4, 5
* Friday: Slot 1, 6

---

### Step 2 — Student Register Review Slot

Sinh viên / Nhóm sinh viên:

* Xem danh sách slot còn trống
* Đăng ký slot review dựa trên availability của giảng viên

---

### Step 3 — Manager Assign Reviewers

Manager thực hiện phân công:

* 2 giảng viên review cho mỗi nhóm
* Dựa trên slot sinh viên đã đăng ký
* Hệ thống validate business rules trước khi assign

---

## Business Rules

### Reviewer Assignment Rules

1. Một nhóm phải có đúng **2 reviewers**
2. Giảng viên **không được review nhóm mình hướng dẫn**
3. Giảng viên phải available tại slot được assign
4. Không vượt quá số buổi review tối đa của giảng viên
5. Cân bằng workload giữa các giảng viên
6. Reviewer không được trùng nhau trong cùng 1 slot

---

## Technical Architecture

## Architecture Style

* RESTful API
* Microservices Architecture
* Clean Architecture
* Repository Pattern
* Dependency Injection
* Docker Ready
* API Gateway Pattern

---

## Proposed Microservices

### Identity Service

Quản lý:

* Authentication / Authorization
* JWT Token
* User Management
* Lecturer / Student / Manager Roles

---

### Session Service

Quản lý:

* Review Sessions
* Import Excel Project / Student Data
* Group / Project Metadata

---

### Availability Service

Quản lý:

* Lecturer Available Slots
* Availability Matrix
* Slot Capacity Calculation

---

### Registration Service

Quản lý:

* Student Slot Registration
* Remaining Slot Capacity
* Registration Validation

---

### Assignment Service

Quản lý:

* Reviewer Assignment
* Business Rule Validation
* Conflict Detection
* Workload Balancing

---

### Report Service

Quản lý:

* Workload Summary
* Dashboard Statistics
* Excel/PDF Export
* Final Timeline Generation

---

## Project Structure

```plaintext
CapstoneReviewSlot/
│
├── ApiGateway/
│   └── CapstoneReviewSlot.Gateway/
│
├── Shared/
│   └── CapstoneReviewSlot.Shared/
│
├── GrpcContract/
│   └── Protos/
│
├── Services/
│   ├── Identity/
│   ├── Session/
│   ├── Availability/
│   ├── Registration/
│   ├── Assignment/
│   └── Report/
│
├── docker-compose.yml
│
└── CapstoneReviewSlot.sln
```

---

## Database Design Overview

### Main Entities

#### User

* Authentication Account

#### Lecturer

* Lecturer Profile
* Separated from User

#### ReviewSession

* Review1 / Review2 / Review3

#### Group

* Student Group / Project

#### LecturerAvailability

* Lecturer Available Slots

#### StudentRegistration

* Student Registered Review Slot

#### ReviewAssignment

* Final Reviewer Assignment

---

## Technology Stack

### Backend

* .NET 8 Web API
* ASP.NET Core
* Entity Framework Core
* SQL Server

### Architecture / Patterns

* Clean Architecture
* CQRS + MediatR
* Repository Pattern
* FluentValidation
* AutoMapper

### Infrastructure

* Docker / Docker Compose
* YARP API Gateway
* gRPC Inter-service Communication

### Logging / Monitoring

* Serilog
* Health Checks

---

## Future Enhancements

* Auto-assignment algorithm for reviewers
* AI/Heuristic workload balancing
* Email / Notification Service
* Calendar Integration
* Admin Dashboard UI
* Export to Excel / PDF / ICS Calendar

---

## Expected Outcomes

Sau khi hoàn thành hệ thống:

* Giảm >80% thời gian tổng hợp lịch review
* Loại bỏ lỗi nhập liệu thủ công
* Tự động enforce business rules
* Dễ dàng mở rộng cho nhiều kỳ review / môn học khác
* Sẵn sàng production / cloud deployment

---

## Development Notes

### MVP Suggestion

Nếu thời gian hạn chế:

#### Phase 1

Build 3 Services:

* Identity Service
* Session Service
* ReviewSlot Service (gộp Availability + Registration + Assignment)

#### Phase 2

Refactor tách thành full microservices

---

## Author

Capstone Project / Graduation Thesis
FPT University Software Engineering
