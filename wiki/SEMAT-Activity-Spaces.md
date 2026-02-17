# Espacios de Actividad — Mapa del Proyecto

Los espacios de actividad de ESSENCE definen las areas de trabajo en el desarrollo de software. A continuacion se documenta como cada fase de SistForm cubrio estos espacios.

> *Este mapa se ha ido completando progresivamente desde v1.0 hasta v2.8. Cada fase se agrego a la documentacion una vez completada.*

| Simbolo | Significado |
|---------|-------------|
| ✓ | Completado en esta fase |
| ◐ | Parcialmente cubierto |
| — | No aplica / No cubierto |

---

### Fase 1: Fundamentos (v1.0 — v1.9)

| Espacio de Actividad | Estado | Evidencia |
|---------------------|--------|-----------|
| **Explorar Posibilidades** | ✓ | Analisis de necesidad en el anteproyecto |
| **Entender Necesidades de Stakeholders** | ✓ | Documentacion de requisitos en introduccion |
| **Entender los Requisitos** | ✓ | Definicion de funcionalidades base (login, registro) |
| **Dar Forma al Sistema** | ◐ | Arquitectura inicial: .NET MAUI + ASP.NET Core |
| **Implementar el Sistema** | ✓ | Login, Signin, HomePage, CreateForm base |
| **Probar el Sistema** | — | Sin pruebas automatizadas |
| **Desplegar el Sistema** | — | Sin despliegue |
| **Planear el Trabajo** | ✓ | Organizacion en versiones semanticas |
| **Coordinar Actividad** | ✓ | Commits atomicos por cada cambio |
| **Seguir Progreso** | ✓ | Tags por version |
| **Identificar y Gestionar Riesgos** | ◐ | Seleccion de tecnologias conocidas |

---

### Fase 2: Formularios (v2.0 — v2.9)

| Espacio de Actividad | Estado | Evidencia |
|---------------------|--------|-----------|
| **Explorar Posibilidades** | — | Fase concluida |
| **Entender Necesidades de Stakeholders** | ◐ | Feedback implicito en iteraciones |
| **Entender los Requisitos** | ✓ | Refinamiento de requisitos de formularios |
| **Dar Forma al Sistema** | ✓ | Arquitectura API REST + DB relacional |
| **Implementar el Sistema** | ✓ | FormsController, FillSurveyPage, modelos |
| **Probar el Sistema** | — | Sin pruebas automatizadas |
| **Desplegar el Sistema** | ◐ | Configuracion de plataforma Android |
| **Planear el Trabajo** | ✓ | Versionado continuo |
| **Coordinar Actividad** | ✓ | Commits atomicos |
| **Seguir Progreso** | ✓ | Tags por version |
| **Identificar y Gestionar Riesgos** | ◐ | Soporte para multiplataforma (Android) |

---

## Cobertura Total por Espacio

| Espacio de Actividad | F1 | F2 | F3 | F4 |
|---------------------|:--:|:--:|:--:|:--:|
| Explorar Posibilidades | ✓ | — | — | — |
| Entender Necesidades de Stakeholders | ✓ | ◐ | — | — |
| Entender los Requisitos | ✓ | ✓ | — | — |
| Dar Forma al Sistema | ◐ | ✓ | — | — |
| Implementar el Sistema | ✓ | ✓ | — | — |
| Probar el Sistema | — | — | — | — |
| Desplegar el Sistema | — | ◐ | — | — |
| Planear el Trabajo | ✓ | ✓ | — | — |
| Coordinar Actividad | ✓ | ✓ | — | — |
| Seguir Progreso | ✓ | ✓ | — | — |
| Identificar y Gestionar Riesgos | ◐ | ◐ | — | — |

---

> Basado en el estandar **ESSENCE v1.2** adaptado al proyecto SistForm.
>
> **Referencia:** Anteproyecto de Trabajo de Graduacion — Cesar Andres Terrado Gonzalez (2023). Mapa generado progresivamente desde v1.0 hasta v2.8.