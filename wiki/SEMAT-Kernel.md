# Kernel ESSENCE — Definicion para SistForm

El estandar **ESSENCE** (Essential Software Engineering Common Terminology) define un kernel universal con 7 alfas (entidades esenciales) que permiten medir el progreso y la salud de cualquier proyecto de software.

A continuacion se define cada alfa para el contexto de SistForm, con sus estados y checklist de progresion.

> *Esta pagina se ha ido construyendo progresivamente desde v1.0 hasta v2.6, refinando la definicion de cada alfa a medida que el proyecto avanzaba.*

---

## 1. Opportunity (Oportunidad)

La necesidad o el problema que motiva el desarrollo del sistema.

### Estados

| Estado | Criterios |
|--------|-----------|
| **Identified** | Se reconoce la falta de una herramienta unificada de recoleccion de datos geoespaciales en Panama |
| **Solution Needed** | Se determina que una plataforma ubicua con soporte offline resuelve el problema |

---

## 2. Stakeholders (Interesados)

Las personas, grupos u organizaciones que afectan o son afectados por el sistema.

### Estados

| Estado | Criterios |
|--------|-----------|
| **Identified** | Se identifican investigadores, tecnicos ambientales y administradores como interesados |
| **Represented** | Los requerimientos de los interesados se documentan en el anteproyecto |
| **Involved** | Los interesados participan en la validacion de funcionalidades |

---

## 3. Requirements (Requisitos)

Lo que el sistema debe hacer para satisfacer a los interesados y la oportunidad.

### Estados

| Estado | Criterios |
|--------|-----------|
| **Conceived** | Idea inicial: sistema de encuestas con autenticacion |
| **Bounded** | Se delimita el alcance: CRUD de formularios, respuestas, usuarios |
| **Coherent** | Los requisitos se organizan en funcionales y no funcionales |

### Requisitos Funcionales Implementados

- RF-01: Autenticacion de usuarios (Login/Registro)
- RF-02: Creacion y edicion de formularios
- RF-03: Respuesta a encuestas
- RF-04: Administracion de formularios (asignacion, gestion)

---

## 4. Software System (Sistema de Software)

El sistema de software que se esta construyendo.

### Estados

| Estado | Criterios |
|--------|-----------|
| **Architecture Selected** | Se elige .NET MAUI (frontend) + ASP.NET Core (backend) + MySQL |
| **Demonstrable** | La autenticacion funciona (Login, Signin, HomePage) |
| **Usable** | Se pueden crear, ver y responder encuestas |

---

## 5. Work (Trabajo)

El conjunto de actividades que se deben realizar para construir el sistema.

### Estados

| Estado | Criterios |
|--------|-----------|
| **Started** | Se inicia el repositorio y la configuracion del proyecto |
| **Under Control** | El trabajo se organiza en versiones con commits atomicos |

### Metricas

| Metrica | Valor |
|---------|-------|
| Versiones | v2.6 |

---

## 6. Team (Equipo)

El grupo de personas responsables de construir el sistema.

### Estados

| Estado | Criterios |
|--------|-----------|
| **Seeded** | Se asigna Cesar Terrado como unico desarrollador |
| **Formed** | Se definen roles y herramientas de desarrollo |

### Roles

| Rol | Responsable |
|-----|-------------|
| Desarrollador Full-Stack | Cesar Terrado |
| Asesor | Dr. Juan Jose Saldana |
| Stakeholder | Investigadores / Tecnicos ambientales |

---

## 7. Way of Working (Modo de Trabajo)

La metodologia, practicas y herramientas que el equipo utiliza para desarrollar el software.

### Estados

| Estado | Criterios |
|--------|-----------|
| **Principles Established** | Se adopta SEMAT/ESSENCE como metodologia base |
| **Foundation Established** | Se define el kernel, las alfas y los espacios de actividad |

### Herramientas

| Herramienta | Proposito |
|-------------|-----------|
| .NET MAUI | Framework frontend multiplataforma |
| ASP.NET Core | Framework backend REST API |
| Entity Framework Core | ORM para base de datos |
| MySQL | Base de datos relacional |
| Git + GitHub | Control de versiones |
| GitHub Wiki | Documentacion del proyecto |

---

> Basado en el estandar **ESSENCE v1.2** (Object Management Group) adaptado al contexto del proyecto SistForm.
>
> **Referencia:** Anteproyecto de Trabajo de Graduacion — Cesar Andres Terrado Gonzalez (2023). Documentacion generada progresivamente desde v1.0 hasta v2.6.