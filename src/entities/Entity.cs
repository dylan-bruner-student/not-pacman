using System.Numerics;
using System.Windows.Controls;
using System.Windows.Shapes;
using WpfApp1;

public abstract class Entity
{
    public Shape SpriteShape = new System.Windows.Shapes.Rectangle();
    protected Vector2 position = Vector2.Zero;

    public abstract void update(double deltaTime);

    public void draw()
    {
        Canvas.SetLeft(SpriteShape, position.X);
        Canvas.SetRight(SpriteShape, position.Y);
    }

    public void destroy()
    {
        MainWindow.entities.Remove(this);
        MainWindow.canvas.Children.Remove(SpriteShape);
    }
}