namespace FgcGames.Domain.Entities;

public class TaskItemExample
{
    public int Id { get; private set; }
    public string Title { get; private set; } = null!;
    public bool IsCompleted { get; private set; }

    public TaskItemExample(string title)
    {
        SetTitle(title);
    }

    public void UpdateTitle(string title) => SetTitle(title);

    public void Complete() => IsCompleted = true;

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title inválido");
        }

        if (title.Length > 100)
        {
            throw new ArgumentException("Title deve ter no máximo 100 caracteres");
        }

        Title = title;
    }
}
