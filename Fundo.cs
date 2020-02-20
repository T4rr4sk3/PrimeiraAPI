using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BD;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace FundoClass
{


	public partial interface IFundoCRUD
	{
		void Criar();

		List<Fundo> ListarTodos();

		void Recuperar(int cod);

		void Alterar(int cod);

		void Deletar();
	}


	public partial class Fundo : IFundoCRUD
	{

		//--Campos

		public Guid Id { get; set; }

		private int codigo;

		public string Nome { get; set; }

		public string Cnpj { get; set; }

		public decimal InvestimentoInicial { get; set; }

		[JsonIgnore]
		public bool Ativo { get; }


		//--Métodos

		//Métodos para serializar e deserializar esse objeto


		//retorna serializado o fundo
		public string Serializar()
		{

			JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true };
			
			string str = JsonSerializer.Serialize(this , GetType() , opt );

			return str;
		}


		//retorna serializado a lista de fundos
		public static string Serializar(List<Fundo> lista)
		{
			JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true };

			string str = JsonSerializer.Serialize(lista, lista.GetType(), opt);

			return str;
		}


		//retorna deserializado o fundo
		public static Fundo Deserializar(string JsonString, Type type)
		{
			Fundo fundo;
			try
			{
				fundo = (Fundo)JsonSerializer.Deserialize(JsonString, type);
			}
			catch(JsonException e)
			{
				throw e;
			}
			
			return fundo;
		}


		//retorna deserializado a lista de fundos
		public static List<Fundo> Deserializar(string JsonString)
		{
			List<Fundo> lista;
			try
			{
				lista = JsonSerializer.Deserialize<List<Fundo>>(JsonString);
			}
			catch(JsonException e)
			{
				throw e;
			}

			return lista;
		}


		//Método para evitar cnpj com caracteres indesejados vindos de qualquer lugar.
		static string TrimCnpj(string cnpj)
		{
			//tira os pontos
			cnpj = cnpj.Replace(".", "").ToLower();
			//tira os "/"
			cnpj = cnpj.Replace("/", "").ToLower();
			//tira o "-"
			cnpj = cnpj.Replace("-", "").ToLower();

			//retorna tirando os espaços dos lados
			return cnpj.Trim();
		}


		// Criar o Fundo no banco de dados.
		public void Criar()
		{
			DBC db = new DBC("BD");
			
			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM dbo.Fundo WHERE id = @_id", db.Con);
				cm.Parameters.AddWithValue("@_id", Id.ToString());

				SqlDataReader dr = cm.ExecuteReader();

				cm.Dispose();

				//verificação se retornou algum valor com mesmo id
				if (!(dr.HasRows))
				{
					dr.Close();

					cm = new SqlCommand("SELECT * FROM dbo.Fundo WHERE cnpj = @_cnpj", db.Con);
					cm.Parameters.AddWithValue("@_cnpj", TrimCnpj(Cnpj));

					dr = cm.ExecuteReader();

					cm.Dispose();

					//verificação se retornou algum valor com mesmo cnpj
					if (!(dr.HasRows))
					{
						dr.Close();

						cm = new SqlCommand("INSERT INTO Fundo (id,nome,cnpj,investInicial) VALUES (@_id , @_nome , @_cnpj , @_invIni)", db.Con);

						cm.Parameters.AddWithValue("@_id", Id.ToString());
						cm.Parameters.AddWithValue("@_nome", Nome);
						cm.Parameters.AddWithValue("@_cnpj", Cnpj);
						cm.Parameters.AddWithValue("@_invIni", InvestimentoInicial);

						cm.ExecuteNonQuery();

					}
				}

				dr.DisposeAsync();

			}
			catch(SqlException e)
			{
				throw e;
			}
			finally
			{
				db.Close();
			}
		}


		//Recuperar Fundo do banco de dados a partir do id em string.
		public void Recuperar(Guid id)
		{
			DBC db = new DBC("BD");

			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM Fundo WHERE id = @_id", db.Con);
				
				cm.Parameters.AddWithValue("@_id", id.ToString().Trim());

				SqlDataReader dr =  cm.ExecuteReader();

				cm.Dispose();

				if (dr.Read())
				{
					this.Id = Guid.Parse((string) dr["id"]);
					this.Cnpj = (string) dr["cnpj"];
					this.codigo = (int) dr["codigo"];
					this.InvestimentoInicial = (decimal) dr["investInicial"];
					this.Nome = (string) dr["nome"];
				}

				dr.DisposeAsync();
			}
			catch(SqlException e)
			{
				throw e;
			}
			catch(FormatException e)
			{
				throw e;
			}
			finally
			{
				db.Close();
			}
		}


		//Recuperar Fundo a partir do codigo do banco de dados para facilitar testes (não é possível alterar o codigo de forma natural, somente pegar)
		public void Recuperar(int codigo)
		{
			DBC db = new DBC("BD");

			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM Fundo WHERE codigo = @_cod", db.Con);

				cm.Parameters.AddWithValue("@_cod", codigo);

				SqlDataReader dr = cm.ExecuteReader();

				cm.Dispose();

				if (dr.Read())
				{
					this.Id = Guid.Parse((string) dr["id"]);
					this.Cnpj = (string) dr["cnpj"];
					this.codigo = (int) dr["codigo"];
					this.InvestimentoInicial = (decimal) dr["investInicial"];
					this.Nome = (string) dr["nome"];
				}

				dr.DisposeAsync();
			}
			catch (SqlException e)
			{
				throw e;
			}
			catch (FormatException e)
			{
				throw e;
			}
			finally
			{
				db.Close();
			}
		}


		//Altera o Fundo por seu codigo(se ele for maior que 0 e existir) no banco de dados, mudando todos os demais atributos.
		public void Alterar(int cod)
		{
			DBC db = new DBC("BD");
			try
			{
				db.Open();

				if (cod > 0)
				{
					SqlCommand cm = new SqlCommand("UPDATE Fundo SET nome = @_nome, cnpj = @_cnpj, investInicial = @_invIni WHERE cod = @_cod", db.Con);

					cm.Parameters.AddWithValue("@_nome", Nome);
					cm.Parameters.AddWithValue("@_cnpj", TrimCnpj(Cnpj));
					cm.Parameters.AddWithValue("@_invIni", InvestimentoInicial);
					cm.Parameters.AddWithValue("@_cod", cod);

					cm.ExecuteNonQuery();

				}

			}
			catch (SqlException e)
			{
				throw e;
			}
			finally
			{
				db.Close();
			}
		}


		//Alterar pelo id do tipo guid
		public void Alterar()
		{
			DBC db = new DBC("BD");

			try
			{
				db.Open();

					SqlCommand cm = new SqlCommand("UPDATE Fundo SET nome = @_nome, cnpj = @_cnpj, investInicial = @_invIni WHERE id = @_id", db.Con);

					cm.Parameters.AddWithValue("@_nome", Nome);
					cm.Parameters.AddWithValue("@_cnpj", TrimCnpj(Cnpj));
					cm.Parameters.AddWithValue("@_invIni", InvestimentoInicial);
					cm.Parameters.AddWithValue("@_id", Id.ToString().Trim());

					cm.ExecuteNonQuery();

			}
			catch (SqlException e)
			{
				throw e;
			}
			finally
			{
				db.Close();
			}
		}


		//Deletar o Fundo por seu codigo no banco de dados.
		public void Deletar()
		{
			DBC db = new DBC("BD");
			try
			{
				db.Open();

				Recuperar(Id);

				if (codigo > 0)
				{
					SqlCommand cm = new SqlCommand("DELETE FROM Fundo WHERE codigo = @_cod", db.Con);

					cm.Parameters.AddWithValue("@_cod",codigo);

					cm.ExecuteNonQuery();

				}

			}
			catch(SqlException e)
			{
				throw e;
			}
			catch(FormatException e) // lançado por Recuperar(Guid Id);
			{
				throw e;
			}
			finally
			{
				db.Close();
			}
		}


		//Deletar pelo codigo no banco de dados;
		public void Deletar(int cod)
		{
			DBC db = new DBC("BD");
			try
			{
				db.Open();

				if (codigo > 0)
				{
					SqlCommand cm = new SqlCommand("DELETE FROM Fundo WHERE id = @_id", db.Con);

					cm.Parameters.AddWithValue("@_cod", cod);

					cm.ExecuteNonQuery();

				}

			}
			catch (SqlException e)
			{
				throw e;
			}
			finally
			{
				db.Close();
			}
		}


		//Listar todos os Fundos
		public List<Fundo> ListarTodos()
		{
			List<Fundo> Fundos = new List<Fundo>();

			DBC db = new DBC("BD");
			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM Fundo WHERE ativo = @_True",db.Con);

				cm.Parameters.AddWithValue("@_True", "True");

				SqlDataReader dr = cm.ExecuteReader();

				cm.Dispose();

				if (dr.HasRows)
				{
					while (dr.Read())
					{
						Fundos.Add(new Fundo
						{
							Id = Guid.Parse( (string) dr["id"]),
							codigo = (Int32) dr["codigo"],
							Nome = (string) dr["nome"],
							Cnpj = (string) dr["cnpj"],
							InvestimentoInicial = (decimal) dr["investInicial"]
						});
					}

					return Fundos;
				}

			} 
			catch(SqlException e)
			{
				throw e;
			}
			catch(FormatException e)
			{
				throw e;
			}
			finally
			{
				db.Close();
			}

			return null;
		}

		public Fundo()	{	}

		//Para facilitar testes, não é possível incluir este no banco de dados por seu codigo começar com 0
		public Fundo(string id, string nome, string cnpj, decimal investInicial)
		{
			Id = Guid.Parse(id.Trim());
			Nome = nome;
			Cnpj = TrimCnpj(cnpj);
			InvestimentoInicial = investInicial;
		}

		public Fundo (int codigo)
		{
			this.codigo = codigo;
			try
			{
				Recuperar(codigo);// pode lançar Sql e Format Exception
			}
			catch (SqlException e)
			{
				throw e;
			}
			catch (FormatException e)
			{
				throw e;
			}
		}
	}
}