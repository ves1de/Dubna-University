namespace Defuser
{
    //Класс который определяет сложность уровня и имеет размер бордера
    public class DifficultyLevel
    {
        public string Name;
        public int BoardSize;
        public override string ToString()
        {
            return Name;
        }
    }
}
