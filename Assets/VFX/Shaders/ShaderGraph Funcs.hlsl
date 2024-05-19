
void PointPerpendicular_float(float2 In, float m, float a, float b, out float2 Out)
{
    Out = float2(In.x, b);

    if (m != 0)
    {
        float m2 = -1 / m;
        Out.x = In.y - m * a - b - m2 * In.x;
        Out.x /= m - m2;
        Out.y = m * (Out.x + a) + b;
    }
}


void Lineal_float(float2 In, float m, float a, float b, float diference, out float Out)
{
    float2 cercano;
    
    PointPerpendicular_float(In, m, a, b, cercano);

    if (distance(In, cercano) < diference)
        Out = 1;
    else
        Out = 0;
}


void LinealPoint_float(float2 In, float2 start, float2 end, float diference, out float Out)
{
    float maxX = end.x > start.x ? end.x : start.x;
    float minX = start.x > end.x ? end.x : start.x;
    float maxY = end.y > start.y ? end.y : start.y;
    float minY = start.y > end.y ? end.y : start.y;
    
    if (In.x > maxX + diference / 1.25 || In.x < minX - diference / 1.25 || In.y < minY - diference / 1.25 || In.y > maxY + diference / 1.25)
    {
        Out = 0;
        return;
    }
    
    if (start.x != end.x)
    {        
        float m = (end.y - start.y) / (end.x - start.x);
        float b = start.y - m * start.x;
        Lineal_float(In, m, (start.y - b) / m - start.x, b, diference, Out);
        return;
    }
    
    start.y = In.y;
    
    if (distance(In, start) < diference)
        Out = 1;
    else
        Out = 0;
}

void Bezier_float(float2 In, float2 start, float2 mid, float2 end, float diference, out float Out)
{    
    // Inicializamos el resultado en 0 (fuera de la curva)
    Out = 0.0;

    // Estimación inicial de t
    float t = 0.5;
    float minDistance = diference;

    // Evaluamos la distancia en varios valores de t para encontrar el mínimo
    [unroll] // Desenrollamos el bucle para optimización
    for (int i = 0; i < 100; i++)
    {
        float oneMinusT = 1.0 - t;
        float2 B = oneMinusT * oneMinusT * start + 2.0 * oneMinusT * t * mid + t * t * end;
        float dist = distance(In, B);

        if (dist < minDistance)
        {
            minDistance = dist;
            Out = 1.0;
            return;
        }

        // Ajustamos t para mejorar la estimación
        t += 0.01; // Incremento de t para próximas evaluaciones
        if (t > 1.0)
            t -= 1.0; // Reiniciar t si excede 1.0
    }
}

void Bezier2_float(float2 In, float2 start, float mid, float2 end, float diference, out float Out)
{
    //Out = 0.0;

    // Cálculo de las coordenadas del punto medio
    float2 midpoint = 0.5 * (start + end);

    // Calcula la pendiente de la línea de start a end
    float m = (end.y - start.y) / (end.x - start.x);
    float b = start.y - m * start.x;
    
    //m = -1 / m;

    // Punto en la perpendicular desde el punto medio
    float2 perpendicularPoint;
    PointPerpendicular_float(midpoint, m, (start.y - b) / m - start.x, b, perpendicularPoint);

    // Desplazamiento en la dirección perpendicular
    float2 direction = normalize(midpoint - perpendicularPoint);
    float2 controlPoint = midpoint + direction * mid;

    //Correfir midpoint
    
    Bezier_float(In, start, midpoint, end, diference, Out);
}



void Path_float(float2 In, float2 start, float2 mid, float2 end, float diference, out float Out)
{
    float resultado;
    float resultado2;
    
    LinealPoint_float(In, start, mid, diference, resultado);
  
    
    LinealPoint_float(In, mid, end, diference, resultado2);
    
    Out = resultado + resultado2;
}

