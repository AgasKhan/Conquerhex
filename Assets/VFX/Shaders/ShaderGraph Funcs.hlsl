
void Lineal_float(float2 In, float m, float a, float b, float diference, out float Out)
{
    float2 cercano = float2(In.x, b);

    if (m != 0)
    {
        float m2 = -1 / m;
        cercano.x = In.y - m * a - b - m2 * In.x;
        cercano.x /= m - m2;
        cercano.y = m * (cercano.x + a) + b;
    }

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

void Path_float(float2 In, float2 start, float2 mid, float2 end, float diference, out float Out)
{
    float resultado;
    float resultado2;
    
    LinealPoint_float(In, start, mid, diference, resultado);
  
    
    LinealPoint_float(In, mid, end, diference, resultado2);
    
    Out = resultado + resultado2;
}

