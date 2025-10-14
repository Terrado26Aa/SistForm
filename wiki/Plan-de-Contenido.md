# Plan de Contenido — Estructura de la Tesis

Estructura propuesta para el desarrollo del trabajo de graduacion **"Analisis, Diseno y Desarrollo de una Plataforma Ubicua para el Levantamiento de Datos de Campo Geoespaciales Utilizando la Metodologia SEMAT"** .

---

## Capitulo 1 — Introduccion

Marco conceptual y contextual del proyecto.

| Seccion | Descripcion |
|---------|-------------|
| **1.1 Marco Teorico del Sistema** | Contexto institucional (SENACYT, STRI, CATHALAC), biodiversidad de Panama y necesidad de herramientas de recoleccion de datos geoespaciales |
| 1.1.1 Antecedentes | Revision de 10 trabajos relacionados de Ecuador, Mexico y Panama |
| 1.1.2 Definicion del problema | Ausencia de una herramienta unificada y adaptable para recoleccion de datos de campo en Panama |
| 1.1.3 Justificacion | Beneficios de la representacion de datos en mapas y la captura movil en sitio |
| 1.1.4 Alcance | Limites y cobertura del proyecto |
| 1.1.5 Limitaciones | Restricciones identificadas |
| **1.2 Marco Teorico de Ingenieria de Software y SEMAT** | Fundamentos teoricos de la metodologia de desarrollo |
| 1.2.1 Modelos de Procesos de Desarrollo | Prescriptivos vs. Agiles — ventajas y desventajas |
| 1.2.2 SEMAT (Software Engineering Method and Theory) | Kernels, Alfas, espacios de actividad, elementos del nucleo |
| 1.2.3 Estandar ESSENCE | Aplicacion practica de SEMAT en el proyecto |

---

## Capitulo 2 — Analisis y Modelado del Sistema

Aplicacion de SEMAT al analisis y diseno de SistForm.

| Seccion | Descripcion |
|---------|-------------|
| **2.1 Estructura del Proceso bajo SEMAT** | Definicion de alfas y espacios de actividad para el proyecto |
| **2.2 Requerimientos del Sistema** | Especificacion funcional y no funcional |
| 2.2.1 Requisitos Funcionales | Autenticacion, creacion de encuestas, respuesta offline, geolocalizacion, sincronizacion |
| 2.2.2 Requisitos no Funcionales | Rendimiento, escalabilidad, seguridad, usabilidad |
| **2.3 Arquitectura y Diseno** | Estructura general de la solucion |
| 2.3.1 Ecosistema de Desarrollo | .NET MAUI (frontend), ASP.NET Core (backend), MySQL/SQLite, Entity Framework Core |
| 2.3.2 Modelado de Comportamiento | Diagramas de casos de uso, secuencia, actividades |
| 2.3.3 Modelo de Datos y Base de Datos | Entidades, relaciones, migraciones |
| 2.3.4 Diseno de Interfaces Visuales | Prototipos y diseno de pantallas XAML |

---

## Capitulo 3 — Desarrollo, Pruebas e Implementacion

Construccion del software siguiendo el plan trazado.

| Seccion | Descripcion |
|---------|-------------|
| **3.1 Codificacion y Construccion** | Implementacion por modulos siguiendo la arquitectura definida |
| 3.1.1 Backend API | Controladores, servicios, autenticacion JWT |
| 3.1.2 Frontend MAUI | Vistas XAML, ViewModels, navegacion |
| 3.1.3 Soporte Offline | SQLite local, cola de sincronizacion |
| 3.1.4 Geolocalizacion | Captura de coordenadas, formatos GeoJSON |
| **3.2 Pruebas del Sistema** | Pruebas unitarias, integracion, aceptacion |
| **3.3 Resultados** | Evaluacion del producto final, cumplimiento de objetivos |

---

## Capitulo 4 — Conclusion

- Logros alcanzados
- Lecciones aprendidas (aplicacion de SEMAT, desarrollo multiplataforma)
- Trabajo futuro y mejoras potenciales

---

## Capitulo 5 — Referencias Bibliograficas

- Minimo 10 referencias actualizadas (ultimos 5 anos)
- Formato academico segun normas de la UTP
- Incluye: MiAmbiente, SENACYT, STRI, CATHALAC, ESRI, y los 10 antecedentes analizados

---

> **Fuente:** Anteproyecto de Trabajo de Graduacion — Cesar Andres Terrado Gonzalez (2023)
