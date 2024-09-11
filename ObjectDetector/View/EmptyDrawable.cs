namespace ObjectDetector.View
{
    public class EmptyDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect) { }
    }
}
