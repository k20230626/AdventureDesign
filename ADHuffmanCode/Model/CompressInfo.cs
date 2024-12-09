namespace ADHuffmanCode.Model;

public class CompressInfo {
    public CompressInfo(double raito, int saveBit) {
        Raito = raito;
        SaveBit = saveBit;
    }
    public double Raito { get; set; }
    public int SaveBit { get; set; }
}