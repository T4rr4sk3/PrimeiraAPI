using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using BD;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace MovimentoClass
{
    public class Movimento
    {
		//--Campos

        public Guid Id { get; set; }

        public Guid IdFundo { get; set; }

        public enum TipoOperacao { Aplicacao = 1, Resgate };

		[JsonPropertyName("tipoOperacao")]
		public string Operacao { get; set; }

        public string CpfCliente { get; set; }

        public decimal ValorMov { get; set; }

		public DateTime DataMov { get; set; }

		//Métodos

		//Nome auto-indicativo, converter a string do banco pra datetime
		static DateTime ConverterStringParaDate(string str)
		{
			DateTime d = new DateTime();

			str = str.Remove(str.Length - 5);
			str = str.Replace("T", " ");

			try
			{
				d = Convert.ToDateTime(str);
			}
			catch(FormatException e)
			{
				throw e;
			}

			return d;
		}


		//Nome auto-indicativo, converter o datetime para uma string
		static string ConverterDateParaString(DateTime d)
		{
			string str;

			try
			{
				str = d.ToString();
			}
			catch (ArgumentOutOfRangeException e)
			{
				throw e;
			}

			str = str.Replace(" ", "T") + "-3:00"; //Como estamos aqui, sempre será GTM -3:00 
			str = str.Replace("/", "-").ToUpper();

			return str;
		}


		//Métodos para serializar e deserializar esse objeto(não sei como criar um metodo para classes genéricas)
		//retorna o objeto serializado.
		public string Serializar()
		{
			JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true };

			string str = JsonSerializer.Serialize(this,GetType(),opt);

			return str;
		}


		//retorna a lista serializada
		public static string Serializar(List<Movimento> lista)
		{
			JsonSerializerOptions opt = new JsonSerializerOptions { WriteIndented = true };

			string str = JsonSerializer.Serialize(lista, lista.GetType(), opt);

			return str;
		}


		//retorna deserializado o movimento
		public static Movimento Deserializar(string JsonString, Type type)
		{
			Movimento movimento;
			try
			{
				movimento = (Movimento)JsonSerializer.Deserialize(JsonString, type);
			}
			catch (JsonException e)
			{
				throw e;
			}

			return movimento;
		}


		//retorna deserializado a lista de movimentos
		public static List<Movimento> Deserializar(string JsonString)
		{
			List<Movimento> lista;
			try
			{
				lista = JsonSerializer.Deserialize<List<Movimento>>(JsonString);
			}
			catch (JsonException e)
			{
				throw e;
			}

			return lista;
		}


		//listar todos os movimentos, sejam eles aplicações ou resgates
		public List<Movimento> ListarTodos()
        {
            List<Movimento> Movimentos = new List<Movimento>();

			DBC db = new DBC("BD");
			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM Movimento", db.Con);

				SqlDataReader dr = cm.ExecuteReader();

				cm.Dispose();

				if (dr.HasRows)
				{
					while (dr.Read())
					{
						Movimentos.Add(new Movimento
						{
							Id = Guid.Parse((string) dr["id"]),
							IdFundo = Guid.Parse((string) dr["idFundo"]),
							CpfCliente = (string) dr["cpfCliente"],
							ValorMov = (decimal) dr["valorMovimentacao"],
							DataMov = ConverterStringParaDate((string) dr["dataMovimentacao"]),
							Operacao = ((int) dr["tipoOperacao"] == 1 ? "Aplicacao":"Resgate")
						});
					}

					return Movimentos;
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


			return Movimentos;
        }


		//listar todos os movimentos de aplicação
		public List<Movimento> ListarAplicacao()
		{
			List<Movimento> Movimentos = new List<Movimento>();

			DBC db = new DBC("BD");
			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM Movimento where tipoOperacao = @_tipo", db.Con);

				cm.Parameters.AddWithValue("@_tipo", Convert.ToInt32(TipoOperacao.Aplicacao));

				SqlDataReader dr = cm.ExecuteReader();

				cm.Dispose();

				if (dr.HasRows)
				{
					while (dr.Read())
					{
						Movimentos.Add(new Movimento
						{
							Id = Guid.Parse((string)dr["id"]),
							IdFundo = Guid.Parse((string)dr["idFundo"]),
							CpfCliente = (string)dr["cpfCliente"],
							ValorMov = (decimal)dr["valorMovimentacao"],
							DataMov = ConverterStringParaDate((string)dr["dataMovimentacao"]),
							Operacao = "Aplicacao"
						});
					}

					return Movimentos;
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


			return Movimentos;
		}


		//listar todos os movimentos de resgate
		public List<Movimento> ListarResgate()
		{
			List<Movimento> Movimentos = new List<Movimento>();

			DBC db = new DBC("BD");
			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM Movimento where tipoOperacao = @_tipo", db.Con);

				cm.Parameters.AddWithValue("@_tipo", Convert.ToInt32(TipoOperacao.Resgate));

				SqlDataReader dr = cm.ExecuteReader();

				cm.Dispose();

				if (dr.HasRows)
				{
					while (dr.Read())
					{
						Movimentos.Add(new Movimento
						{
							Id = Guid.Parse((string)dr["id"]),
							IdFundo = Guid.Parse((string)dr["idFundo"]),
							CpfCliente = (string)dr["cpfCliente"],
							ValorMov = (decimal)dr["valorMovimentacao"],
							DataMov = ConverterStringParaDate((string)dr["dataMovimentacao"]),
							Operacao = "Resgate"
						});
					}

					return Movimentos;
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


			return Movimentos;
		}

		//deletar um movimento
		public void Deletar()
		{
			DBC db = new DBC("BD");

			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("DELETE FROM Movimento WHERE id = @_id", db.Con);

				cm.Parameters.AddWithValue("@_id", Id.ToString());

				cm.ExecuteNonQuery();

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


		//salvar um movimento no banco de dados
		public void Criar()
		{
			DBC db = new DBC("BD");

			try
			{
				db.Open();

				SqlCommand cm = new SqlCommand("SELECT * FROM Fundo where id = @_id", db.Con);

				//Serie de if's para verificar se está tudo pelo menos preenchido
				if ((IdFundo != Guid.Empty) && (IdFundo != null))
				{

					cm.Parameters.AddWithValue("@_id", IdFundo);

					SqlDataReader dr = cm.ExecuteReader();

					cm.Dispose();

					if (dr.HasRows)
					{
						
						dr.Close();

						if ((Operacao.ToLower() == "aplicacao") || (Operacao.ToLower() == "resgate"))
						{

							if (CpfCliente != null)
							{

								if (ValorMov > 0)
								{
									cm = new SqlCommand("INSERT INTO Movimento (id, idFundo, tipoOperacao, cpfCliente, valorMovimentacao, dataMovimentacao) values (@_id, @_idFundo, @_tipo, @_cpf, @_valor, @_data)",db.Con);

									cm.Parameters.AddWithValue("@_id", Id.ToString());
									cm.Parameters.AddWithValue("@_idFundo", IdFundo.ToString());
									cm.Parameters.AddWithValue("@_tipo", (Operacao.ToLowerInvariant() == "aplicacao") ? Convert.ToInt32(TipoOperacao.Aplicacao) : Convert.ToInt32(TipoOperacao.Resgate));
									cm.Parameters.AddWithValue("@_cpf", CpfCliente);
									cm.Parameters.AddWithValue("@_valor", ValorMov);
									cm.Parameters.AddWithValue("@_data", ConverterDateParaString(DataMov));

									cm.ExecuteNonQuery();

								}
							}
						}
					}
				}
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


		//construtores
		public Movimento() { }

		public Movimento(string id, string idFundo, string cpfCliente, decimal valor, string data, string tipoOp)
		{
			Id = Guid.Parse(id);
			IdFundo = Guid.Parse(idFundo);
			CpfCliente = cpfCliente;
			ValorMov = valor;
			DataMov = ConverterStringParaDate(data);
			Operacao = tipoOp;
		}

    }
}
