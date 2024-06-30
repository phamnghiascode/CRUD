using System.Data.Common;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using TodoList.Utilities;
namespace TodoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadTodos();
            
        }
        private void LoadTodos()
        {
            //"Server=localhost; Database=TodoDB;User Id=sa; Password=123456; Trusted_Connection=True; TrustServerCertificate=True"
            using (SqlConnection _connection = new SqlConnection(Helper.GetConnectionString("TodoListDB")))
            {

                _connection.Open();
                string query = "SELECT * FROM Todos ORDER BY DueDate";
                SqlDataAdapter adapter = new SqlDataAdapter(query, _connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                todosDataGrid.ItemsSource = dataTable.DefaultView;
            }
        }

        private void AddTodo_Click(object sender, RoutedEventArgs e)
        {
            string title = titleTextBox.Text;
            string description = descriptionTextBox.Text;
            DateTime dueDate = dueDatePicker.SelectedDate ?? DateTime.Now;

            using (SqlConnection _connection = new SqlConnection(Helper.GetConnectionString("TodoListDB")))
            {
                _connection.Open();
                string query = "INSERT INTO Todos (Title, Description, DueDate) VALUES (@Title, @Description, @DueDate)";
                SqlCommand command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Title", title);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@DueDate", dueDate);
                command.ExecuteNonQuery();
            }

            LoadTodos();
            ClearForm();
        }

        private void MarkAsDone_Click(object sender, RoutedEventArgs e)
        {
            var selected = todosDataGrid.SelectedItem as DataRowView;
            if (selected != null)
            {
                int id = (int)selected["Id"];

                using (SqlConnection _connection = new SqlConnection(Helper.GetConnectionString("TodoListDB")))
                {
                    _connection.Open();
                    string query = "UPDATE Todos SET IsDone = 1 WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, _connection);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                LoadTodos();
            }
        }

        private void ClearForm()
        {
            titleTextBox.Text = "";
            descriptionTextBox.Text = "";
            dueDatePicker.SelectedDate = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var selected = todosDataGrid.SelectedItem as DataRowView;
            if (selected != null)
            {
                int id = (int)selected["Id"];

                using (SqlConnection _connection = new SqlConnection(Helper.GetConnectionString("TodoListDB")))
                {
                    _connection.Open();
                    string query = "DELETE FROM Todos WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, _connection);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                LoadTodos();
            }
        }
    }
}