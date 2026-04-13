using FgcGames.Domain.Entities;

namespace FgcGames.Domain.Interfaces.Repositories;

public interface ITaskItemExampleRepository
{
    /// <summary>
    /// Adiciona uma nova task ao contexto.
    /// </summary>
    /// <param name="taskItem">Task a ser adicionada.</param>
    void Add(TaskItemExample taskItem);

    /// <summary>
    /// Atualiza uma task existente no contexto.
    /// </summary>
    /// <param name="taskItem">Task com os dados atualizados.</param>
    void Update(TaskItemExample taskItem);

    /// <summary>
    /// Remove uma task do contexto.
    /// </summary>
    /// <param name="taskItem">Task a ser removida.</param>
    void Delete(TaskItemExample taskItem);

    /// <summary>
    /// Obtém uma task pelo seu identificador.
    /// </summary>
    /// <param name="id">Identificador da task.</param>
    /// <returns>
    /// A task encontrada ou <c>null</c> caso não exista.
    /// </returns>
    Task<TaskItemExample?> GetByIdAsync(int id);

    /// <summary>
    /// Obtém todas as tasks cadastradas.
    /// </summary>
    /// <returns>Lista somente leitura de tasks.</returns>
    Task<IReadOnlyList<TaskItemExample>> GetAllAsync();

    /// <summary>
    /// Verifica se já existe uma task com o título informado.
    /// </summary>
    /// <param name="title">Título da task a ser verificado.</param>
    /// <returns>
    /// <c>true</c> caso já exista uma task com o mesmo título; caso contrário, <c>false</c>.
    /// </returns>
    Task<bool> ExistsByTitleAsync(string title);

    /// <summary>
    /// Persiste no banco de dados todas as alterações pendentes no contexto atual,
    /// incluindo inserções, atualizações e remoções.
    /// </summary>
    /// <returns>
    /// Quantidade de registros afetados.
    /// </returns>
    Task<int> SaveChangesAsync();
}
