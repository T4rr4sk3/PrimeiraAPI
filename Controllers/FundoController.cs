using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FundoClass;
using System.Data.SqlClient;

namespace Tentativa2APIviaJSON.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FundoController : ControllerBase
    {
        private const string Contact = "\nSe necessário, contate o Administrador do sistema.";

        // GET: Fundo , retorna uma lista que contém todos os fundos 
        [HttpGet]
        public IActionResult Get()
        {
            List<Fundo> fundos;
            try
            {
                fundos = new Fundo().ListarTodos();//pode lançar SQLException e FormatException

                if (fundos.Count < 1)
                {
                    return null;
                }

                return Ok(Fundo.Serializar(fundos));

            }
            catch(SqlException e)
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch(FormatException e)
            {
                return UnprocessableEntity(e.Message + Contact);
            }
        }

        // GET: Fundo/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Fundo fundo = new Fundo(id);
                if (string.IsNullOrEmpty(fundo.Nome))
                {
                    return null;
                }

                return Ok(fundo.Serializar());
            }
            catch(SqlException e)
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (FormatException e)
            {
                return UnprocessableEntity(e.Message + Contact);
            }
        }

        // POST: Fundo
        [HttpPost]
        public IActionResult Post([FromBody] JsonElement j)
        {
            try
            {
                string str = j.ToString();

                if (str.StartsWith("["))
                {
                    List<Fundo> lista = Fundo.Deserializar(str);

                    for (int i = 0; i < lista.Count; i++)
                    {
                        if ((lista[i].Nome != null) && (lista[i].Cnpj != null))
                            lista[i].Criar();
                    }
                }
                else
                {
                    Fundo f = Fundo.Deserializar(str, new Fundo().GetType());

                    if ((f.Nome != null) && (f.Cnpj != null))
                        f.Criar();
                }

                return Ok();
            }
            catch (SqlException e) // Exceção lançada pelo "Fundo".Criar
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (JsonException e) // Exceção lançada pelo "Fundo".Deserializar
            {
                return BadRequest(e.Message + Contact);
            }
        }

        // PUT: Fundo
        [HttpPut]
        public IActionResult Put([FromBody] JsonElement j)
        {
            try {
                string str = j.ToString();
                if (str.StartsWith("["))
                {
                    List<Fundo> lista = Fundo.Deserializar(str);

                    for (int i = 0; i < lista.Count; i++)
                    {
                        if((lista[i].Nome != null) && (lista[i].Cnpj != null))
                            lista[i].Alterar();
                    }
                }
                else
                {
                    Fundo fundo = Fundo.Deserializar(str, new Fundo().GetType());

                    if ((fundo.Nome != null) && (fundo.Cnpj != null))
                        fundo.Alterar();
                }
            
                return Ok(str);
            }
            catch (SqlException e) // Exceção lançada pelo "Fundo".Alterar
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (JsonException e) // Exceção lançada pelo "Fundo".Deserializar
            {
                return BadRequest(e.Message + Contact);
            }
        }

        // DELETE: Fundo
        [HttpDelete]
        public IActionResult Apagar([FromBody] JsonElement j)
        {
            try
            {
                string str = j.ToString();
                if (str.StartsWith("["))
                {
                    List<Fundo> l = Fundo.Deserializar(str);

                    for (int i = 0; i < l.Count; i++)
                    {
                        l[i].Deletar();
                    }
                }
                else
                {
                    Fundo f = Fundo.Deserializar(str, new Fundo().GetType());
                    f.Deletar();
                }

                return Ok("Deletado com sucesso.");
            }
            catch (SqlException e) // Exceção lançada pelo "Fundo".Deletar
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (JsonException e) // Exceção lançada pelo "Fundo".Deserializar
            {
                return BadRequest(e.Message + Contact);
            }
        }

    }
}
