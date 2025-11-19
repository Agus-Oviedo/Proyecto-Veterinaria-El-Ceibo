Veterinaria El Ceibo – Sistema de Gestión

Aplicación web para la gestión integral de una veterinaria, desarrollada con ASP.NET Core MVC y Entity Framework Core.

Permite administrar clientes, mascotas, turnos, historia clínica, internaciones y libreta sanitaria, con control de acceso por roles.

---

Funcionalidades principales

 **Autenticación y usuarios**
  - Registro y login de usuarios con **ASP.NET Core Identity**
  - Gestión de usuarios y asignación de roles desde una pantalla solo para administrador.

- **Roles**
  - **Administrador**
    - Puede acceder a todas las secciones.
    - Puede ver la lista de usuarios y asignar roles.
  - **Veterinario**
    - Gestiona clientes y mascotas.
    - Carga **consultas clínicas** (historia clínica).
    - Gestiona **internaciones** (hoja de internación).
    - Gestiona **libreta sanitaria** (vacunas y desparasitaciones).
    - Gestiona turnos.
  - **Peluquería**
    - Puede gestionar **clientes**, **mascotas** y **turnos**.
    - **No** puede acceder a historia clínica, internaciones ni libreta sanitaria.

- **Clientes y Mascotas**
  - ABM de clientes (tutores).
  - ABM de mascotas asociadas a cada tutor.
  - Cálculo de edad, castrado/sin castrar, observaciones, etc.

- **Turnos**
  - Agenda mensual tipo calendario.
  - Asignación de turno a **cliente + mascota**.
  - Tipos de turno: **Atención veterinaria** / **Peluquería**.
  - Cambio rápido de estado del turno (Pendiente, Confirmado, Atendido, Cancelado) mediante **Stored Procedure**.
  - Visualización de turnos de un día para evitar superposiciones (en crear/editar).

- **Historia clínica**
  - Registro de consultas clínicas por mascota.
  - Campos como motivo, anamnesis, examen físico, diagnóstico, tratamiento, indicaciones, próximo control.
   - Asociación de consulta con un **veterinario** (seleccionado desde combo).

- **Internaciones**
  - Iniciar internación a partir de una consulta clínica.
  - Hoja de internación con:
    - Fecha de ingreso, motivo, estado (Activa / Alta).
    - Registros de evolución: descripción, peso, temperatura, veterinario.
  - Dar de alta una internación con fecha y bloqueo de nuevos registros.

- **Plan / libreta sanitaria**
  - Registro de **vacunaciones** y **desparasitaciones** por mascota.
  - Cada registro con fecha, producto/vacuna y veterinario.

- **UI / UX**
  - Layout con **navbar** y estilos personalizados de la veterinaria.
  - Botones de **“Volver”** unificados en formato “pastilla” para facilitar la navegación.
  - Notificaciones con **Toastr** (éxito / error / eliminación).
  - Confirmaciones de borrado con **SweetAlert2**.

---

Tecnologías utilizadas

- **.NET** (ASP.NET Core MVC)
- **Entity Framework Core** (Code First)
- **ASP.NET Core Identity** (con `ApplicationUser`)
- **SQL Server**
- **Bootstrap / Bootswatch (Litera)** + CSS personalizado
- **jQuery**
- **Toastr** (notificaciones)
- **SweetAlert2** (diálogos de confirmación)

---

Requisitos previos

Para poder ejecutar el proyecto necesitás acceder a: http://elceibovet.runasp.net/


