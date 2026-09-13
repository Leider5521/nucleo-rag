# nucleo-rag

Motor de recuperación híbrida y generación trazable para consultar, en lenguaje natural, documentación empresarial heterogénea (PDF, XLSX, DOCX y formatos antiguos), con respuestas siempre citadas a su documento de origen.

> Proyecto académico en desarrollo — Proyecto I, Especialización en Inteligencia Artificial, Corporación Unificada Nacional de Educación Superior (CUN), Equipo G9. Estado actual: **diseño y prototipo**.

## El problema

Las organizaciones acumulan su conocimiento operativo en cientos de archivos con estructuras muy distintas: manuales, reglamentos, procedimientos, matrices y registros repartidos entre carpetas, áreas y formatos. Encontrar una respuesta depende de conocer la carpeta correcta, el nombre aproximado del archivo o el área responsable, y distintas personas terminan consultando fuentes diferentes para la misma pregunta.

Usar un modelo generativo por sí solo tampoco resuelve el problema: puede producir contenido plausible que no está respaldado por el repositorio. Lo que hace falta es una capa que conecte preguntas en lenguaje natural con la documentación real y entregue respuestas que el usuario pueda verificar frente al documento original.

## Qué hace este proyecto

`nucleo-rag` implementa una arquitectura RAG (Retrieval-Augmented Generation) diseñada específicamente para repositorios documentales empresariales:

- **Ingesta multiformato.** Lee PDF (con texto digital o escaneado, aplicando OCR cuando hace falta), hojas de cálculo, documentos de Word y formatos antiguos que requieren conversión previa, conservando metadatos de procedencia (nombre, área, ruta lógica, fecha de modificación).
- **Índice híbrido.** Cada fragmento tiene dos representaciones: una semántica (embeddings) para preguntas formuladas con vocabulario distinto al del documento, y una léxica (BM25) para códigos, nombres propios y expresiones exactas. Los resultados de ambas se combinan y reordenan antes de llegar al modelo.
- **Generación trazable.** El modelo generativo recibe únicamente la evidencia recuperada, responde con base en ella, cita las fuentes utilizadas y reconoce abiertamente cuándo la información disponible es insuficiente.
- **Ciclo de vida documental.** Actualización incremental del índice, detección de duplicados, registro de cambios y retiro del contenido que deja de pertenecer al repositorio autorizado, para no responder con versiones obsoletas.
- **Procesamiento local.** Archivos, vectores y consultas se procesan dentro de infraestructura controlada por la organización, reduciendo la transferencia de información corporativa a servicios externos.
- **Arquitectura modular.** Los modelos de lenguaje, de embeddings y el motor vectorial son intercambiables mediante interfaces, para poder compararlos con preguntas reales del dominio y elegir por calidad, memoria, velocidad y compatibilidad con el hardware disponible.

## Arquitectura

El sistema se organiza en cuatro capas:

1. **Fuentes documentales autorizadas** — carpetas y repositorios desde los que se incorporan los documentos.
2. **Ingesta** — lectura, conversión, OCR, normalización, segmentación en fragmentos con contexto suficiente y creación de metadatos.
3. **Indexación y recuperación** — almacenamiento de representaciones vectoriales y estructuras léxicas; búsqueda híbrida y reordenamiento.
4. **Orquestación de generación** — recibe la pregunta ya filtrada por autorización, construye el contexto a partir de los fragmentos recuperados, invoca al modelo generativo y entrega la respuesta con sus citas y documentos de origen.

```
Documentos ──▶ Ingesta ──▶ Índice híbrido (embeddings + BM25)
                                    │
Pregunta ──▶ Filtros de autorización ──▶ Recuperación ──▶ Generación con citas ──▶ Respuesta verificable
```

La autenticación de usuarios y los permisos por rol o nivel de confidencialidad **no** forman parte de este componente: los resuelve un módulo complementario de control de acceso, que entrega al motor los filtros de autorización ya resueltos antes de cada consulta.

## Estructura del repositorio

```
nucleo-rag/
├── ingesta/        # lectores por formato, OCR, segmentación, metadatos
├── indexacion/     # embeddings, índice léxico, actualización incremental
├── recuperacion/   # búsqueda híbrida y reordenamiento
├── generacion/     # construcción de contexto, prompts, citas
├── evaluacion/     # banco de preguntas y métricas de recuperación/generación
├── config/         # parámetros de modelos, rutas y umbrales
├── docs/           # documento del proyecto y decisiones de diseño
└── README.md
```

La estructura puede cambiar a medida que avance el prototipo.

## Evaluación

La evaluación separa deliberadamente recuperación y generación, porque cada una puede fallar por razones distintas:

- **Recuperación:** presencia del documento esperado entre los primeros resultados y capacidad de recuperar la evidencia pertinente, sobre un banco de preguntas representativas del dominio.
- **Generación:** fidelidad respecto a la fuente, claridad, completitud, validez de las citas y capacidad de reconocer cuándo la evidencia es insuficiente.
- **Rendimiento:** tiempo de respuesta, consumo de recursos y estabilidad ante concurrencia progresiva.

## Estudio de caso

El motor se desarrolla y valida sobre el repositorio documental real de una empresa de servicios en Cartagena, Colombia: 703 archivos (358 XLSX, 284 PDF, 47 DOCX, 9 XLS, 4 DOC y 1 PPTX) que respaldan procesos administrativos, operativos, jurídicos, de auditoría y de seguridad. **Ningún documento, dato ni credencial de la organización se publica en este repositorio**; el código es genérico y puede aplicarse a cualquier repositorio documental heterogéneo.

## Hoja de ruta

- [ ] Fase 1 — Inventario documental, clasificación por áreas y banco de preguntas representativas
- [ ] Fase 2 — Prototipo técnico: extracción, segmentación, indexación, recuperación y citación
- [ ] Fase 3 — Integración con los filtros de autorización del módulo de control de acceso
- [ ] Fase 4 — Evaluación funcional y de rendimiento mediante piloto controlado

## Requisitos e instalación

Python 3.10 o superior. Las dependencias concretas se irán fijando en `requirements.txt` a medida que se defina el prototipo.

```bash
git clone https://github.com/<usuario>/nucleo-rag.git
cd nucleo-rag
python -m venv .venv
source .venv/bin/activate   # en Windows: .venv\Scripts\activate
pip install -r requirements.txt
```

## Referencias

- Gao, Y. et al. (2023). *Retrieval-augmented generation for large language models: A survey*. arXiv:2312.10997.
- Karpukhin, V. et al. (2020). *Dense passage retrieval for open-domain question answering*. EMNLP 2020.
- Lewis, P. et al. (2020). *Retrieval-augmented generation for knowledge-intensive NLP tasks*. NeurIPS 33.
- Reimers, N. y Gurevych, I. (2019). *Sentence-BERT: Sentence embeddings using Siamese BERT-networks*. EMNLP-IJCNLP 2019.
- Robertson, S. y Zaragoza, H. (2009). *The probabilistic relevance framework: BM25 and beyond*. Foundations and Trends in Information Retrieval, 3(4).

## Licencia

Distribuido bajo la licencia MIT. Consulta el archivo `LICENSE`.
