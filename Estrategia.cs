
using System;
using System.Collections.Generic;
using tp1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace tpfinal
{

	public class Estrategia
	{
        private int CalcularDistancia(string str1, string str2)
        {

            // Si cualquiera de las dos cadenas es nula o sólo espacios, devuelvo un valor grande (no coincidencia)
            if (string.IsNullOrWhiteSpace(str1) || string.IsNullOrWhiteSpace(str2))
            {
                return 1000;
            }

            // Saco espacios alrededor y paso todo a minúsculas para comparar sin distinguir mayúsculas
            str1 = str1.Trim().ToLower();
            str2 = str2.Trim().ToLower();

            // Si las cadenas completas son exactamente iguales la distancia es 0
            if (str1 == str2)
            {
                return 0;
            }

            // divido cada cadena en palabras separadas por espacios, evitando entradas vacias
            String[] strlist1 = str1.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            String[] strlist2 = str2.Split(' ', StringSplitOptions.RemoveEmptyEntries);


            //inicializo la distancia con un valor grande para poder tomar el minimo despues
            int distance = 1000;

            //recorro cada palabra del primer texto
            foreach (String s1 in strlist1)
            {
                //recorro cada palabra del segundo texto
                foreach (String s2 in strlist2)
                {
                    //si alguna palabra coincide la distancia es inmediatamente 0
                    if (s1 == s2)
                    {
                        return 0;
                    }

                    //si no coincide exactamente, calculo la distancia de levenshtein entre las dos palabras
                    // y me quedo con la minima encontrada hasta ahora 
                    distance = Math.Min(distance, Utils.calculateLevenshteinDistance(s1, s2));



                }
            }



            return distance;
            //devuelvo la distancia minima encontrada, si no hubo cambios quedará en 1000 y no devuelve nada
            //return distance;
        }




        public String Consulta1(ArbolGeneral<DatoDistancia> arbol)
        {
            //llamo al metodo recursivo para 
            string resultado = recolectarHojas(arbol);

            //devuelve la cadena de texto completa que se generó
            return resultado;


        }

        private string recolectarHojas(ArbolGeneral<DatoDistancia> arbol)
        {

            //se inicializa un string vacío para ir acumulando las hojas encontradas en esta rama
            string hojasEncontradas = "";

            //si el nodo actual es una hoja...
            if (arbol.esHoja())
            {
                //se añade la información del nodo al string y se agrega un salto de linea
                hojasEncontradas += arbol.getDatoRaiz().ToString() + "\n";
            }

            //si el nodo actual no es una hoja...
            else
            {
                //se recorre cada uno de sus hijos
                foreach (ArbolGeneral<DatoDistancia> hijo in arbol.getHijos())
                {
                    //elresultado de la llamada(las hojas encontradas en el subárbol del hijo) se concatena al string "hojasEncontradas"
                    hojasEncontradas += recolectarHojas(hijo);
                }

            }

            //devuelve el string con las hojas encontradas en este nivek del arbol
            return hojasEncontradas;
        }
        


        public String Consulta2(ArbolGeneral<DatoDistancia> arbol)
        {
            // Lista para guardar las líneas de texto de cada camino encontrado.
            List<string> todosLosCaminosEncontrados = new List<string>();

            // Lista para construir el camino actual durante la recursión.
            // Usa 'DatoDistancia' para tener acceso a la distancia de cada nodo.
            List<DatoDistancia> caminoActual = new List<DatoDistancia>();

            // Inicia el proceso recursivo.
            RecolectarCaminos(arbol, caminoActual, todosLosCaminosEncontrados);

            // Une todas las líneas de texto en un solo string final.
            string resultadoFinal = "";
            foreach (string linea in todosLosCaminosEncontrados)
            {
                resultadoFinal += linea + "\n"; //se añade cadacamino 
            }
            return resultadoFinal;
        }

        private void RecolectarCaminos(ArbolGeneral<DatoDistancia> arbol, List<DatoDistancia> caminoActual, List<string> todosLosCaminosEncontrados)
        {
            // 1. Agrega el objeto completo (con texto y distancia) al camino.
            caminoActual.Add(arbol.getDatoRaiz());

            if (arbol.esHoja())
            {
                // 2. Si es hoja, construye el string final para este camino.
                string caminoCompleto = "";
                for (int i = 0; i < caminoActual.Count; i++)
                {
                    DatoDistancia dato = caminoActual[i];

                    // Formatea cada nodo del camino con su distancia guardada.
                    // La distancia de la raíz debe ser 0 para que aparezca como (0).
                    caminoCompleto += "(" + dato.distancia + ") " + dato.texto;

                    // se agrega una coma pero solo si no es el último elemento.
                    if (i < caminoActual.Count - 1)
                    {
                        caminoCompleto += ", ";
                    }
                }
                //se guarda el camino formateado en la lista de resultados finales
                todosLosCaminosEncontrados.Add(caminoCompleto);
            }
            else
            {
                //Si no es hoja, la recursión continúa en cada hijo.
                foreach (ArbolGeneral<DatoDistancia> hijo in arbol.getHijos())
                {
                    RecolectarCaminos(hijo, caminoActual, todosLosCaminosEncontrados);
                }
            }

            //despues de haber visitado un nodo y todos sus descendientes (es decir, al salir de la llamada recursiva),
            //se elimina el nodo actual del camino
            //esto es "como dar un paso atras" para poder explorar otras ramas del arbol 
            //si no hicieramos esto, los camunos de diferentes ramas se mezclarian.
            caminoActual.RemoveAt(caminoActual.Count - 1);
        }
        


        public String Consulta3(ArbolGeneral<DatoDistancia> arbol)
        {
            //si el arbol no existe o esta vacio, devuelve un mensaje y termina
            if (arbol == null || arbol.esVacio())
            {
                return "El árbol está vacío.";
            }

            string resultadoFinal = "";

            //se crea una cola que almacenará los nodos a visitar
            Cola<ArbolGeneral<DatoDistancia>> cola = new Cola<ArbolGeneral<DatoDistancia>>();

            //se encola el primer nodo (la raiz del arbol) parainiciar el proceso
            cola.encolar(arbol);

            //se inicializa un contador para llevar la cuenta del nivel actual
            int nivel = 0;


            //mientras la cola no este vacía...
            while (!cola.esVacia())
            {
                // Se obtiene el numero de nodos que hay actualmente en la cola
                int nodosEnNivel = cola.cantidadElementos();
                resultadoFinal += "Nodos en el Nivel " + nivel + "\n";

                //lista temporal para guardar los textos de los nodos
                List<string> textosDelNivel = new List<string>();


                //este bucle se ejecutará "nodosEnNivel" veces, procesando cada nodo del nivel actual
                for (int i = 0; i < nodosEnNivel; i++)
                {
                    //se saca el siguiente nodo de la cola para procesarlo
                    ArbolGeneral<DatoDistancia> nodoActual = cola.desencolar();
                    DatoDistancia dato = nodoActual.getDatoRaiz();

                    //se formatea el teto del nodo yse guarda en la lista temporal
                    string textoFormateado = "(" + dato.distancia + ") " + dato.texto;
                    textosDelNivel.Add(textoFormateado);

                    //se recorren todos los hijos del nodo actual
                    foreach (ArbolGeneral<DatoDistancia> hijo in nodoActual.getHijos())
                    {
                        // Se encolan
                        cola.encolar(hijo);
                    }
                }

                //se recorre la lista de textos del nivel que acabamos de procesar
                for (int i = 0; i < textosDelNivel.Count; i++)
                {
                    //se agrega cada texto al resultado final
                    resultadoFinal += textosDelNivel[i];

                    //si no es el ultimo elemento se añade una coma y un espacio
                    if (i < textosDelNivel.Count - 1)
                    {
                        resultadoFinal += ", ";
                    }
                }
                resultadoFinal += "\n";

                //se incrementa el contador de nivel para la siguiente iteración
                nivel++;
            }

            //una vez que la cola esta vacia, se han visitado todos los nodos
            //se devuelve el string con el resultado completo

            return resultadoFinal;
        }
        



        // Método que agrega un dato al árbol respetando la distancia calculada
        public void AgregarDato(ArbolGeneral<DatoDistancia> arbol, DatoDistancia dato)
        {
                // Si el árbol es nulo no hago nada
                if (arbol == null)
                {
                    return;
                }

                // Calculo la distancia entre la raiz actual y el nuevo dato a insertar
                int distancia = CalcularDistancia(arbol.getDatoRaiz().texto, dato.texto);

                // Recorro los hijos para ver si ya existe una rama con esa misma distancia
                foreach (var hijo in arbol.getHijos())
                {
                    // Si encuentro un hijo cuya etiqueta de distancia coincide
                    // inserto recursivamente en ese hijo (descenso por la rama)
                    if (hijo.getDatoRaiz().distancia == distancia)
                    {
                        AgregarDato(hijo, dato);
                        return; //Termino porque ya inserté
                    }
                }

                // Si no encontré un hijo con esa distancia, creo un nuevo nodo y lo agrego como hijo
                var datoNuevo = new DatoDistancia(distancia, dato.texto, dato.descripcion);// Creo el DatoDistancia con la distancia al padre
                var nuevoNodo = new ArbolGeneral<DatoDistancia>(datoNuevo);// Creo el nodo del árbol
                arbol.agregarHijo(nuevoNodo);// Lo agrego como hijo de la raíz actual
        }




        




        //metodo que busca en el arbol todos los nodos cuya distancia al texto buscado sea <= umbral
        public void Buscar(ArbolGeneral<DatoDistancia> arbol, string elementoABuscar, int umbral, List<DatoDistancia> collected)
		{
            // Si el árbol o la lista de resultados es nula, no hago nada y retorno
            if (arbol == null || collected == null)
            {
                return;
            }

            // Si el texto a buscar es nulo, lo convierto en cadena vacía para evitar excepciones
            if (elementoABuscar == null)
            {
                elementoABuscar = "";
            }


           


            // Calculo la distancia entre el texto nodo actual y el buscado
            int distancia = CalcularDistancia(arbol.getDatoRaiz().texto, elementoABuscar);
            



            //Si la distancia está dentro del umbral, agrego el resultado
            if (distancia <= umbral)
            {
                var nodo = arbol.getDatoRaiz();

                //agrego una copia de DatoDistancia donde la "distancia" es la distancia respectoa la busqueda
                //esto evita mostrar la distancia almacenada en el nodo, que es la distancai al padre
                collected.Add(new DatoDistancia (distancia, nodo.texto,nodo.descripcion));
            }


            // Recorro recursivamente todos los hijos del nodo actual
            foreach (var hijo in arbol.getHijos())
            {
                Buscar(hijo, elementoABuscar, umbral, collected); // Llamada recursiva para cada hijo
            }
        }


    }
 
    }
    
