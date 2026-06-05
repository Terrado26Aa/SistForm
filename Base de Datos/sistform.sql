-- phpMyAdmin SQL Dump
-- version 5.2.3
-- https://www.phpmyadmin.net/
--
-- Servidor: db:3306
-- Tiempo de generación: 06-05-2026 a las 18:43:32
-- Versión del servidor: 8.0.46
-- Versión de PHP: 8.3.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `sistform`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `createforms`
--

CREATE TABLE `createforms` (
  `IdForm` int NOT NULL,
  `IdUser` int NOT NULL,
  `Title` varchar(255) NOT NULL,
  `Description` text,
  `CreationDate` datetime DEFAULT CURRENT_TIMESTAMP,
  `LastModifiedDate` datetime DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `createforms`
--

INSERT INTO `createforms` (`IdForm`, `IdUser`, `Title`, `Description`, `CreationDate`, `LastModifiedDate`) VALUES
(8, 1, 'Inspección de Infraestructura Vial', 'Registro del estado de calles, aceras y señalización en campo.', '2026-05-06 11:38:37', '2026-05-06 11:38:37'),
(9, 1, 'Satisfacción Ciudadana - Servicios Municipales', 'Percepción de ciudadanos sobre servicios del municipio en su sector.', '2026-05-06 11:38:37', '2026-05-06 11:38:37'),
(10, 1, 'Levantamiento de Árboles Urbanos en Riesgo', 'Registro técnico de árboles en vías públicas con riesgo de caída.', '2026-05-06 11:38:38', '2026-05-06 11:38:38'),
(11, 1, 'Control de Comercios en Vía Pública', 'Verificación legal y sanitaria de puestos y comercios en espacios públicos.', '2026-05-06 11:38:38', '2026-05-06 11:38:38');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `form_assignments`
--

CREATE TABLE `form_assignments` (
  `Id` int NOT NULL,
  `FormId` int NOT NULL,
  `UserId` int NOT NULL,
  `AssignedDate` datetime DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `form_assignments`
--

INSERT INTO `form_assignments` (`Id`, `FormId`, `UserId`, `AssignedDate`) VALUES
(1, 8, 7, '2026-05-06 17:18:14'),
(2, 9, 2, '2026-05-06 17:18:20');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `form_elements`
--

CREATE TABLE `form_elements` (
  `Id` int NOT NULL,
  `IdForm` int NOT NULL,
  `Title` varchar(255) DEFAULT NULL,
  `Type` varchar(50) DEFAULT NULL,
  `Options` text,
  `MaxSelections` int DEFAULT '0'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `form_elements`
--

INSERT INTO `form_elements` (`Id`, `IdForm`, `Title`, `Type`, `Options`, `MaxSelections`) VALUES
(18, 8, 'Estado general del pavimento', 'Seleccion Unica', 'Bueno,Regular,Malo,Muy malo', 0),
(19, 8, 'Tipo de daño visible', 'Checklist', 'Grietas,Baches,Hundimientos,Sin daño', 0),
(20, 8, 'Nivel de severidad (1-10)', 'Campo de Entrada', '', 0),
(21, 8, '¿Hay señalización horizontal visible?', 'Seleccion Unica', 'Sí completa,Sí parcial,No hay', 0),
(22, 8, 'Fotografía del tramo', 'Imagen', '', 0),
(23, 8, 'Observaciones del inspector', 'Campo de Entrada', '', 0),
(24, 9, '¿Con qué frecuencia usa servicios municipales?', 'Seleccion Unica', 'Diariamente,Semanalmente,Mensualmente,Raramente', 0),
(25, 9, 'Califique la recolección de basura', 'Seleccion Unica', 'Excelente,Buena,Regular,Mala', 0),
(26, 9, 'Califique el alumbrado público', 'Seleccion Unica', 'Excelente,Bueno,Regular,Malo', 0),
(27, 9, 'Servicios que necesitan mejora', 'Checklist', 'Agua potable,Recolección de basura,Alumbrado,Parques,Transporte,Seguridad', 0),
(28, 9, 'Comentarios o sugerencias', 'Campo de Entrada', '', 0),
(29, 10, 'Especie del árbol', 'Campo de Entrada', '', 0),
(30, 10, 'Estado fitosanitario', 'Seleccion Unica', 'Sano,Con plagas,Seco parcial,Muerto', 0),
(31, 10, 'Condición de riesgo detectada', 'Checklist', 'Inclinación peligrosa,Raíces levantando acera,Ramas sobre cables,Sin riesgo', 0),
(32, 10, 'Acción recomendada', 'Seleccion Unica', 'Ninguna,Poda preventiva,Poda correctiva,Tala urgente', 0),
(33, 10, 'Fotografía del árbol', 'Imagen', '', 0),
(34, 10, 'Observaciones técnicas', 'Campo de Entrada', '', 0),
(35, 11, 'Nombre del comerciante o negocio', 'Campo de Entrada', '', 0),
(36, 11, 'Tipo de comercio', 'Seleccion Unica', 'Puesto fijo,Carrito ambulante,Toldo provisional,Quiosco', 0),
(37, 11, 'Rubro comercial', 'Seleccion Unica', 'Alimentos,Ropa,Electrónica,Servicios,Artesanías', 0),
(38, 11, '¿Posee permiso municipal vigente?', 'Seleccion Unica', 'Sí,No,No responde', 0),
(39, 11, 'Condiciones sanitarias observadas', 'Checklist', 'Limpieza adecuada,Manejo correcto de residuos,Uso de EPP,No cumple', 0),
(40, 11, 'Fotografía del puesto', 'Imagen', '', 0),
(41, 11, 'Observaciones del inspector', 'Campo de Entrada', '', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `form_responses`
--

CREATE TABLE `form_responses` (
  `Id` int NOT NULL,
  `FormId` int NOT NULL,
  `UserId` int NOT NULL,
  `Date` datetime DEFAULT CURRENT_TIMESTAMP,
  `LatitudeA` decimal(10,8) DEFAULT NULL,
  `LongitudeA` decimal(11,8) DEFAULT NULL,
  `LatitudeB` decimal(10,8) DEFAULT NULL,
  `LongitudeB` decimal(11,8) DEFAULT NULL,
  `RoutePath` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `form_response_details`
--

CREATE TABLE `form_response_details` (
  `Id` int NOT NULL,
  `ResponseId` int NOT NULL,
  `QuestionTitle` varchar(255) DEFAULT NULL,
  `Answer` text
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `users`
--

CREATE TABLE `users` (
  `Id` int NOT NULL,
  `UserName` varchar(255) NOT NULL,
  `FirstName` varchar(255) DEFAULT NULL,
  `LastName` varchar(255) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `PasswordHash` text,
  `Role` varchar(50) DEFAULT 'User'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Volcado de datos para la tabla `users`
--

INSERT INTO `users` (`Id`, `UserName`, `FirstName`, `LastName`, `Email`, `PasswordHash`, `Role`) VALUES
(1, 'cesar.terrado', 'Cesar', 'Terrrado', 'cesar@sistform.com', '$2a$11$AWYLIM2IcXHcZuMfycUVaO/DQYMIPGRk2g5.6QEggX91i65UVKEP6', 'Admin'),
(2, 'angelica.santana', 'Angelica', 'Santana', 'angelica@sistform.com', '$2a$11$eKiqM0512J7VIBsLG2jkwehdMQ1Gjl1RZlQTZRY3ukWbM6P7Rw8/y', 'User'),
(6, 'juan.perez', 'Juan', 'Perez', 'Juan.perez@gmail.com', '$2a$11$5e65ray/gp8SJvX3qvYT8.tl2yedng1Qdnm2s0fNOe5KoNFDokK/q', 'User'),
(7, 'alvaro.samudio', 'Alvaro', 'Samudio', 'alvarolsamudio@gmail.com', '$2a$11$SEY3MMnItG9nWLfxY8p9Ue4yQW//WpWXGue8/btmbfDcb6QNFkiyK', 'User');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `createforms`
--
ALTER TABLE `createforms`
  ADD PRIMARY KEY (`IdForm`),
  ADD KEY `IdUser` (`IdUser`);

--
-- Indices de la tabla `form_assignments`
--
ALTER TABLE `form_assignments`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FormId` (`FormId`),
  ADD KEY `UserId` (`UserId`);

--
-- Indices de la tabla `form_elements`
--
ALTER TABLE `form_elements`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IdForm` (`IdForm`);

--
-- Indices de la tabla `form_responses`
--
ALTER TABLE `form_responses`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `FormId` (`FormId`),
  ADD KEY `UserId` (`UserId`);

--
-- Indices de la tabla `form_response_details`
--
ALTER TABLE `form_response_details`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `ResponseId` (`ResponseId`);

--
-- Indices de la tabla `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `UserName` (`UserName`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `createforms`
--
ALTER TABLE `createforms`
  MODIFY `IdForm` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=12;

--
-- AUTO_INCREMENT de la tabla `form_assignments`
--
ALTER TABLE `form_assignments`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `form_elements`
--
ALTER TABLE `form_elements`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=42;

--
-- AUTO_INCREMENT de la tabla `form_responses`
--
ALTER TABLE `form_responses`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=30;

--
-- AUTO_INCREMENT de la tabla `form_response_details`
--
ALTER TABLE `form_response_details`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `users`
--
ALTER TABLE `users`
  MODIFY `Id` int NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `createforms`
--
ALTER TABLE `createforms`
  ADD CONSTRAINT `createforms_ibfk_1` FOREIGN KEY (`IdUser`) REFERENCES `users` (`Id`);

--
-- Filtros para la tabla `form_assignments`
--
ALTER TABLE `form_assignments`
  ADD CONSTRAINT `form_assignments_ibfk_1` FOREIGN KEY (`FormId`) REFERENCES `createforms` (`IdForm`),
  ADD CONSTRAINT `form_assignments_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`);

--
-- Filtros para la tabla `form_elements`
--
ALTER TABLE `form_elements`
  ADD CONSTRAINT `form_elements_ibfk_1` FOREIGN KEY (`IdForm`) REFERENCES `createforms` (`IdForm`);

--
-- Filtros para la tabla `form_responses`
--
ALTER TABLE `form_responses`
  ADD CONSTRAINT `form_responses_ibfk_1` FOREIGN KEY (`FormId`) REFERENCES `createforms` (`IdForm`),
  ADD CONSTRAINT `form_responses_ibfk_2` FOREIGN KEY (`UserId`) REFERENCES `users` (`Id`);

--
-- Filtros para la tabla `form_response_details`
--
ALTER TABLE `form_response_details`
  ADD CONSTRAINT `form_response_details_ibfk_1` FOREIGN KEY (`ResponseId`) REFERENCES `form_responses` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
