
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

    [unroll] 
    for (int i = 0; i < 100; i++)
    {
        float oneMinusT = 1.0 - t;
        float2 B = oneMinusT * oneMinusT * start + 2.0 * oneMinusT * t * mid + t * t * end;
        float dist = distance(In, B);

        if (dist < minDistance)
        {
            Out = 1.0;
            return;
        }

        t += 0.01; 
        if (t > 1.0)
            t -= 1.0; // Reiniciar t si excede 1.0
    }
}

void Bezier2_float(float2 In, float2 start, float curvatura, float2 end, float diference, out float Out)
{
    Out = 0.0;
    float2 midpoint = 0.5 * (start + end);

    // Vector dirección de la línea de start a end
    float2 direction = end - start;
    float2 perpendicularDirection = normalize(float2(-direction.y, direction.x)); // Perpendicular a la línea

    // Punto de control, desplazado desde el punto medio en la dirección perpendicular
    float2 controlPoint = midpoint + perpendicularDirection * distance(start,end) * curvatura;
    
    
    Bezier_float(In, start, controlPoint, end, diference, Out);
}



void Path_float(float2 In, float2 start, float2 mid, float2 end, float diference, out float Out)
{
    float resultado;
    float resultado2;
    
    LinealPoint_float(In, start, mid, diference, resultado);
  
    
    LinealPoint_float(In, mid, end, diference, resultado2);
    
    Out = resultado + resultado2;
}

