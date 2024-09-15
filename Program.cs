using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace MantisAutomation
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IWebDriver driver = new ChromeDriver())
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                try
                {
                    // Acessar a página de login do Mantis
                    driver.Navigate().GoToUrl("http://mantis-prova.base2.com.br/login_page.php");

                    // Testar Login
                    TestLogin(driver, wait, "Thiago_Pereira", "$paceCowboy2022");

                    // Testar criação de bug
                    TestCreateBug(driver, wait);
                    
                    // Testar edição de bug
                    TestEditBug(driver, wait);
                    
                    // Testar logout
                    TestLogout(driver, wait);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro durante os testes: " + ex.Message);
                }
                finally
                {
                    driver.Quit();
                }
            }
        }

        static void TestLogin(IWebDriver driver, WebDriverWait wait, string username, string password)
        {
            try
            {
                // Esperar até que o campo de nome de usuário esteja presente e preencher
                IWebElement usernameField = wait.Until(d => d.FindElement(By.Id("username")));
                usernameField.SendKeys(username);

                // Esperar até que o botão de login esteja presente e clicável
                IWebElement submitButtonUser = wait.Until(d => d.FindElement(By.CssSelector("input[type='submit']")));
                submitButtonUser.Click();
                
                // Esperar pela presença do campo de senha
                WaitForPageToLoad(driver, wait, By.Id("password"));

                // Preencher o campo de senha
                IWebElement passwordField = driver.FindElement(By.Id("password"));
                passwordField.SendKeys(password);

                // Esperar até que o botão de login esteja presente e clicável
                IWebElement submitButtonPassword = driver.FindElement(By.CssSelector("input[type='submit']"));
                submitButtonPassword.Click();

                // Verificar se a página foi carregada com sucesso após o login
                WaitForPageToLoad(driver, wait, By.Id("breadcrumbs"));
                
                Console.WriteLine("Login realizado com sucesso.");
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("Elemento não encontrado durante o login: " + ex.Message);
            }
        }

        static void TestCreateBug(IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                // Navegar para a página de criação de bug
                IWebElement reportIssueLink = wait.Until(d => d.FindElement(By.LinkText("Report Issue")));
                reportIssueLink.Click();

                // Esperar pela presença dos campos de criação de bug
                WaitForPageToLoad(driver, wait, By.Id("summary"));

                // Preencher o formulário de bug
                var comboBox = wait.Until(d => d.FindElement(By.Id("category_id")));

                // Crie uma instância do SelectElement
                var selectElement = new SelectElement(comboBox);

                // Selecione uma opção pelo texto visível
                selectElement.SelectByText("[All Projects] categoria teste");
                
                driver.FindElement(By.Id("summary")).SendKeys("Bug de Teste");
                driver.FindElement(By.Id("description")).SendKeys("Descrição do bug de teste.");

                // Alterar a forma de identificar o botão
                // Aqui estamos tentando encontrar o botão de criação com uma combinação de identificadores ou class names
                IWebElement createBugButton = wait.Until(d => d.FindElement(By.CssSelector("input[type='submit']")));
                createBugButton.Click();
                
            
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("Elemento não encontrado durante a criação do bug: " + ex.Message);
            }
        }

        static void TestEditBug(IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                // Navegar para a lista de bugs e editar um bug existente
                IWebElement viewIssuesLink = driver.FindElement(By.CssSelector("a[href='/view_all_bug_page.php']"));
                viewIssuesLink.Click();

                // Aguarda o carregamento do botão "Print Reports"
                WaitForPageToLoad(driver, wait, By.LinkText("Print Reports"));

                // Encontrar o primeiro item da tabela e seleciona-lo para editar o bug 
                // Encontre todos os elementos <td> com a classe "column-id"
                var tdElements = driver.FindElements(By.CssSelector("td.column-id"));

                // Verifique se pelo menos um elemento foi encontrado
                if (tdElements.Count > 0)
                {
                    // Clique no primeiro elemento encontrado
                    tdElements[0].Click();

                    // Adicione um tempo de espera ou uma verificação após o clique
                    System.Threading.Thread.Sleep(4000); // Espera 4 segundos

                }
                else
                {
                    // Caso nenhum elemento seja encontrado, exiba uma mensagem de erro
                    System.Console.WriteLine("Nenhum elemento <td> com a classe 'column-id' foi encontrado.");
                }

                //Adicionar uma tag ao Bug
                // Preencher o formulário de bug
                var comboBoxTag = wait.Until(d => d.FindElement(By.Id("tag_select")));

                // Crie uma instância do SelectElement
                var selectElement = new SelectElement(comboBoxTag);

                // Selecione uma opção pelo texto visível
                selectElement.SelectByText("bug");

                // Encontrar e clicar no botão "Attach"
                IWebElement attachButton = driver.FindElement(By.CssSelector("input[type='submit'][value='Attach']"));
                attachButton.Click();

                //Adicionar nota
                IWebElement textArea = wait.Until(driver => 
                {
                    return driver.FindElement(By.Id("bugnote_text"));
                });

                // Envie o texto para o <textarea>
                textArea.SendKeys("Atualização Bugnote");

                // Opcional: Adicione um tempo de espera ou uma verificação após o envio
                System.Threading.Thread.Sleep(2000); // Espera 2 segundos
                
                // Encontra a <div> pela classe
                var dropzoneDiv = wait.Until(drv => drv.FindElement(By.CssSelector("div.dropzone.center.dz-clickable")));

                // Clica na <div> para abrir o diálogo de upload
                dropzoneDiv.Click();

                // Aguarda um breve momento para garantir que o diálogo de upload foi aberto
                System.Threading.Thread.Sleep(1000);

                // Encontra o <input> do tipo file, que pode ter sido inserido dinamicamente ou estar presente
                var fileInput = driver.FindElement(By.CssSelector("input[type='file']"));

                // Caminho para o arquivo que você deseja fazer upload
                string filePath = @"C:\Users\Thiago\Downloads\Thiago_Pereira´s_Projec.csv"; //Alterar o local em que o arquivo se encontra

                // Envia o caminho do arquivo para o input
                fileInput.SendKeys(filePath);

                // Aguarda um breve momento para garantir que o upload foi realizado
                System.Threading.Thread.Sleep(3000);
                
                //Encontra e clica no botão "add note"
                IWebElement addNoteButton = driver.FindElement(By.CssSelector("input[type='submit'][value='Add Note']"));
                addNoteButton.Click();

                //Retorna a tela View Issues
                System.Threading.Thread.Sleep(5000);
                IWebElement returnViewIssuesLink = driver.FindElement(By.CssSelector("a[href='/view_all_bug_page.php']"));
                returnViewIssuesLink.Click();

                // Aguarda o carregamento do botão "Print Reports"
                WaitForPageToLoad(driver, wait, By.LinkText("Print Reports"));
        
                
                //Testa Funcionamento "CSV Exports"
                // Encontrar e clicar no botão CSV Export
                IWebElement exportCSVLink = wait.Until(d => d.FindElement(By.LinkText("CSV Export")));
                exportCSVLink.Click();

                // Aguardar o download (ajuste o tempo conforme necessário)
                Thread.Sleep(5000);

                // Verificar se o arquivo foi baixado
                string filePathCSV = @"C:\Users\Thiago\Downloads\Thiago_Pereira´s_Projec.csv"; //Alterar o local onde o arquivo sera salvo para teste em outro local
                if (File.Exists(filePathCSV))
                {
                Console.WriteLine("Arquivo baixado CSV com sucesso.");
                }
                else
                {
                Console.WriteLine("O arquivo CSV não foi encontrado.");
                }

                //Testa Funcionamento "Excel Export"
                // Encontrar e clicar no botão Excel Export
                IWebElement exportExcelLink = wait.Until(d => d.FindElement(By.LinkText("Excel Export")));
                exportExcelLink.Click();

                // Aguardar o download (ajuste o tempo conforme necessário)
                Thread.Sleep(5000);

                // Verificar se o arquivo foi baixado
                string filePathXML = @"C:\Users\Thiago\Downloads\Thiago_Pereira´s_Projec.xml"; //Alterar o local onde o arquivo sera salvo para teste em outro local
                if (File.Exists(filePathXML))
                {
                Console.WriteLine("Arquivo XML baixado com sucesso.");
                }
                else
                {
                Console.WriteLine("O arquivo XML não foi encontrado.");
                }

                //Testa Funcionamento "Print Reports"
                driver.FindElement(By.LinkText("Print Reports")).Click();
                WaitForPageToLoad(driver, wait, By.Id("buglist"));
                
                // Alterar detalhes do bug adicionando nota
                
                IWebElement bugnoteField = driver.FindElement(By.Id("bugnote_text"));
                bugnoteField.Click();
                bugnoteField.SendKeys("Nota Bug de Teste Atualizado");
                driver.FindElement(By.CssSelector("input[type='submit']")).Click();

                
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("Elemento não encontrado durante a edição do bug: " + ex.Message);
            }
        }

        static void TestLogout(IWebDriver driver, WebDriverWait wait)
        {
            try
            {
                //Navegar para pagina inicial
                IWebElement myViewLink = driver.FindElement(By.CssSelector("a[href='/my_view_page.php']"));
                myViewLink.Click();
                System.Threading.Thread.Sleep(5000);
                
                // Navegar para o logout
                // Encontra e expande o dropdown
                IWebElement dropdownElement = driver.FindElement(By.CssSelector("a[data-toggle='dropdown']"));
                dropdownElement.Click();
                IWebElement logoutLink = wait.Until(d => d.FindElement(By.CssSelector("a[href='/logout_page.php']")));
                logoutLink.Click();

                // Esperar pela presença do campo de nome de usuário na página de login
                WaitForPageToLoad(driver, wait, By.Id("username"));
                Console.WriteLine("Logout realizado com sucesso.");
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine("Elemento não encontrado durante o logout: " + ex.Message);
            }
        }

        // Função auxiliar para aguardar o carregamento da página
        static void WaitForPageToLoad(IWebDriver driver, WebDriverWait wait, By by)
        {
            wait.Until(d => d.FindElement(by));
        }
    }
}
