# Mini Database Mapper

Mini Database Mapper es una herramienta que simplifica la creación de consultas SQL al permitir a los programadores construir consultas de bases de datos de manera programática en lugar de escribirlas manualmente en SQL.

Los beneficios clave de usar esta herramienta incluyen la reducción de errores sintácticos y lógicos en las consultas, mayor legibilidad y mantenibilidad del código, mayor productividad al escribir consultas complejas, facilitación de la adaptación a cambios en la estructura de la base de datos, y la reducción de la curva de aprendizaje para nuevos desarrolladores, lo que en conjunto mejora la eficiencia y calidad del desarrollo de aplicaciones que requieren interacción con bases de datos.

### Uso

Para interactuar con una base de datos utilizando operaciones CRUD (Crear, Leer, Actualizar y Eliminar) en C# utilizando un ORM (Mapeo Objeto-Relacional) sigue estos pasos generales:

1. Crear (Create): Para agregar un nuevo juguete a la base de datos, debes usar la operación "Create". Debes proporcionar el nombre y el precio del nuevo juguete como parámetros, como se muestra en el ejemplo: db.Toys.Create(new Toy { Name = "nombre_del_juguete", Price = precio_del_juguete });.

2. Eliminar (Delete): Si deseas eliminar un juguete existente de la base de datos, utiliza la operación "Delete". Debes especificar un criterio para identificar el juguete que deseas eliminar. En el ejemplo, se busca un juguete con un valor de ID igual a 0 y se elimina: db.Toys.Delete(x => x.Id == 0);.

3. Leer (Find): Para recuperar información de juguetes de la base de datos, utiliza la operación "Find". Debes proporcionar un criterio de búsqueda, como un valor de ID, para obtener los juguetes que coincidan con ese criterio. En el ejemplo, se buscan juguetes con un valor de ID igual a 0 y se almacenan en la variable toys.

4. Actualizar (Update): Si necesitas modificar la información de un juguete existente en la base de datos, puedes utilizar la operación "Update". Debes proporcionar los nuevos valores que deseas asignar al juguete y un criterio para identificar el juguete que se actualizará. En el ejemplo, se cambia el nombre a "nombre_modificado" y el precio a 500 para el juguete con un valor de ID igual a 0: db.Toys.Update(new { Name = "nombre_modificado", Price = 500 }, t => t.Id == 0);.
