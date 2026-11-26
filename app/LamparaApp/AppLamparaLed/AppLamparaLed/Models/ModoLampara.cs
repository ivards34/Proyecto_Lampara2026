namespace AppLamparaLed.Models;

public enum ModoLampara
{
    Full = 1,
    LuzTenue = 2,
    LamparaTemperatura = 3,
    Apagado = 4
}

public static class ModoLamparaExtensions
{
    public static string Texto(this ModoLampara modo) => modo switch
    {
        ModoLampara.Full => "Full",
        ModoLampara.LuzTenue => "Luz Tenue",
        ModoLampara.LamparaTemperatura => "Lámpara Temperatura",
        _ => "Apagado"
    };

    public static ModoLampara DesdeTexto(this string? texto) => texto switch
    {
        "Full" => ModoLampara.Full,
        "Luz Tenue" => ModoLampara.LuzTenue,
        "Lámpara Temperatura" => ModoLampara.LamparaTemperatura,
        _ => ModoLampara.Apagado
    };

    public static ModoLampara DesdeEstado(this string? estado) =>
        int.TryParse(estado, out var id) && Enum.IsDefined(typeof(ModoLampara), id)
            ? (ModoLampara)id
            : ModoLampara.Apagado;
}
