# 🏥 Sistema de Pacientes y Historial Clínico

Sistema de **microservicios** para la gestión de pacientes e historiales clínicos, desarrollado con **.NET 8, Docker Compose, RabbitMQ y SQL Server**.

## 🏗️ Arquitectura

El sistema está compuesto por:

* **API Gateway:** punto de entrada principal y encargado de enrutar las peticiones.
* **Pacientes API:** gestión de pacientes.
* **Historial API:** gestión de historiales clínicos.
* **RabbitMQ:** comunicación asíncrona entre microservicios.
* **SQL Server:** almacenamiento de datos.

```text
                    CLIENTE
                       │
                       ▼
              API GATEWAY :5001
                  /           \
                 ▼             ▼
       PACIENTES API :5002   HISTORIAL API :5003
                 │             ▲
                 ▼             │
            PacientesDB        │
                 │             │
                 ▼             │
              RabbitMQ ────────┘
                 │
                 ▼
             HistorialDB
```

## 🔌 Puertos

| Servicio            |  Puerto |
| ------------------- | ------: |
| API Gateway         |  `5001` |
| Pacientes API       |  `5002` |
| Historial API       |  `5003` |
| RabbitMQ Management | `15672` |
| RabbitMQ            |  `5672` |

> Los puertos `5001`, `5002` y `5003` son puertos locales publicados mediante `localhost`. No son direcciones IP.

---

## ⚙️ Requisitos

* Docker Desktop
* .NET 8 SDK
* SQL Server
* SQL Server Management Studio (SSMS)
* Git

---

## 🗄️ Base de Datos

El proyecto utiliza dos bases de datos:

* `PacientesDB`
* `HistorialDB`

Los scripts SQL se encuentran en la carpeta `BaseDatos`:

```text
BaseDatos/
├── PacienteDB.sql
└── HistorialDB.sql
```

### Ejecución

Abrir **SQL Server Management Studio (SSMS)** y ejecutar los scripts:

```text
BaseDatos/PacienteDB.sql
BaseDatos/HistorialDB.sql
```

---

## 🚀 Ejecución del proyecto

### 1. Clonar el proyecto

```bash
git clone https://github.com/mateojosue17lozada-spec/DIST-4BM-B3-AD01-DockerCompose-RabbitMQApiGateway-Lozada-Mateo.git
cd proyectoDockercomposeconEDARABBITMQAPIGATEWAY
```

### 2. Crear las bases de datos

Ejecutar en **SQL Server Management Studio**:

```text
BaseDatos/PacienteDB.sql
BaseDatos/HistorialDB.sql
```

### 3. Levantar los contenedores

```bash
docker compose up --build
```

### 4. Verificar los contenedores

```bash
docker compose ps
```

### 5. Ver los logs

```bash
docker compose logs -f
```

---

# 🌐 Acceso a los servicios

## API Gateway

URL:

```text
http://localhost:5001
```

Swagger:

```text
http://localhost:5001/swagger/index.html
```

El API Gateway funciona como punto de entrada para acceder a los microservicios.

---

## Pacientes API

URL:

```text
http://localhost:5002
```

Swagger:

```text
http://localhost:5002/swagger/index.html
```

### Endpoints

```text
GET    /api/Pacientes
GET    /api/Pacientes/{id}
POST   /api/Pacientes
PUT    /api/Pacientes/{id}
DELETE /api/Pacientes/{id}
```

---

## Historial API

URL:

```text
http://localhost:5003
```

Swagger:

```text
http://localhost:5003/swagger/index.html
```

### Endpoints

```text
GET    /api/Historial
GET    /api/Historial/{id}
GET    /api/Historial/paciente/{idPaciente}
POST   /api/Historial
PUT    /api/Historial/{id}
DELETE /api/Historial/{id}
```

---

# 🐇 RabbitMQ

RabbitMQ permite la comunicación asíncrona entre **Pacientes API** y **Historial API**.

### Panel de administración

```text
http://localhost:15672
```

### Credenciales

```text
Usuario: admin
Contraseña: admin123
```

### Puerto de comunicación

```text
5672
```

### Cola utilizada

```text
paciente_creado
```

---

# 🔄 Flujo del sistema

Cuando se crea un nuevo paciente, el flujo es:

```text
1. Cliente
      ↓
2. API Gateway :5001
      ↓
3. Pacientes API :5002
      ↓
4. Guarda el paciente en PacientesDB
      ↓
5. Publica evento en RabbitMQ
      ↓
6. Historial API recibe el evento
      ↓
7. Verifica el paciente
      ↓
8. Crea automáticamente el historial
      ↓
9. Guarda en HistorialDB
```

De esta manera, los microservicios se comunican mediante **RabbitMQ**.

---

# 📁 Estructura del proyecto

```text
proyectoDockercomposeconEDARABBITMQAPIGATEWAY/
│
├── ApiGateway/
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
│
├── Pacientes.Api/
│   ├── Controllers/
│   │   └── PacientesController.cs
│   ├── Data/
│   │   └── PacientesDBContext.cs
│   ├── Models/
│   │   └── Paciente.cs
│   ├── Services/
│   │   └── RabbitMQPublisher.cs
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
│
├── Historial.Api/
│   ├── Controllers/
│   │   └── HistorialController.cs
│   ├── Data/
│   │   └── HistorialDBContext.cs
│   ├── Events/
│   │   └── PacienteCreadoEvento.cs
│   ├── Models/
│   │   └── HistorialClinico.cs
│   ├── Services/
│   │   └── RabbitMQConsumer.cs
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
│
├── BaseDatos/
│   ├── PacienteDB.sql
│   └── HistorialDB.sql
│
├── Scripts/
│   ├── PacientesDB.sql
│   └── HistorialDB.sql
│
├── docker-compose.yml
└── README.md
```

---

# 🧪 Ejemplos de uso

### Crear un paciente

```bash
curl -X POST http://localhost:5001/api/Pacientes \
-H "Content-Type: application/json" \
-d "{\"cedula\":\"7778889990\",\"nombre\":\"Pedro\",\"apellido\":\"Ramirez\",\"direccion\":\"Calle Nueva 123\"}"
```

### Obtener todos los pacientes

```bash
curl http://localhost:5001/api/Pacientes
```

### Obtener historial de un paciente

```bash
curl http://localhost:5001/api/Historial/paciente/1
```

También se pueden realizar estas operaciones desde **Swagger**.

---

# 🛠️ Tecnologías utilizadas

| Tecnología            | Uso                     |
| --------------------- | ----------------------- |
| .NET 8                | Desarrollo de las APIs  |
| ASP.NET Core          | Microservicios          |
| Entity Framework Core | Acceso a datos          |
| SQL Server            | Base de datos           |
| RabbitMQ              | Comunicación asíncrona  |
| YARP                  | API Gateway             |
| Docker                | Contenedores            |
| Docker Compose        | Orquestación            |
| Swagger/OpenAPI       | Documentación y pruebas |

---

# ✅ Resumen

El proyecto implementa una arquitectura de **microservicios** donde:

* **API Gateway** centraliza las peticiones.
* **Pacientes API** administra los pacientes.
* **Historial API** administra los historiales clínicos.
* **RabbitMQ** permite la comunicación entre servicios.
* **SQL Server** almacena la información.
* **Docker Compose** permite ejecutar todos los servicios de forma conjunta.
* **Swagger** permite probar y consultar las APIs.
