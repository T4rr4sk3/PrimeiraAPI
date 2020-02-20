using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MovimentoClass;
using System.Data.SqlClient;


namespace Tentativa2APIviaJSON.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MovimentoController : ControllerBase
    {
        private const string Contact = "\nSe necessário, contate o Administrador do sistema.";

        // GET: Movimento
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Movimento> lista = new Movimento().ListarTodos();
                if (lista.Count < 1)
                {
                    return null;
                }
                return Ok(Movimento.Serializar(lista));
            }
            catch(SqlException e) // lançado por ListarTodos
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch(FormatException e)
            {
                return UnprocessableEntity(e.Message + Contact);
            }
        }


        // GET: Movimento/Resgate
        [HttpGet("{action}",Name = "Resgate")]
        public IActionResult Resgate()
        {
            try
            {
                List<Movimento> lista = new Movimento().ListarResgate();
                if (lista.Count < 1)
                {
                    return null;
                }

                return Ok(Movimento.Serializar(lista));
            }
            catch (SqlException e)  // lançado por ListarResgate
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (FormatException e)
            {
                return UnprocessableEntity(e.Message + Contact);
            }
        }


        // GET: Movimento/Aplicacao
        [HttpGet("{action}", Name = "Aplicacao")]
        public IActionResult Aplicacao()
        {
            try
            {
                List<Movimento> lista = new Movimento().ListarAplicacao();
                if (lista.Count < 1)
                {
                    return null;
                }

                return Ok(Movimento.Serializar(lista));
            }
            catch (SqlException e) // lançado por ListarAplicacao
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (FormatException e)
            {
                return UnprocessableEntity(e.Message + Contact);
            }
        }


        // POST: Movimento
        [HttpPost]
        public IActionResult Post([FromBody] JsonElement json)
        {
            try
            {
                string str = json.ToString();

                if (str.StartsWith("["))
                {
                    List<Movimento> lista = Movimento.Deserializar(str);

                    for (int i = 0; i < lista.Count; i++)
                    {
                        lista[i].Criar();
                    }
                }
                else
                {
                    Movimento movimento = Movimento.Deserializar(str, new Movimento().GetType());

                    movimento.Criar();
                }

                return Ok();
            }
            catch (SqlException e) // Exceção lançada pelo "Movimento".Criar
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (JsonException e) // Exceção lançada pelo "Movimento".Deserializar
            {
                return BadRequest(e.Message + Contact);
            }
        }

        
        // DELETE: Movimento
        [HttpDelete]
        public IActionResult Delete([FromBody] JsonElement json)
        {
            try
            {
                string str = json.ToString();
                if (str.StartsWith("["))
                {
                    List<Movimento> lista = Movimento.Deserializar(str);

                    for (int i = 0; i < lista.Count; i++)
                    {
                        lista[i].Deletar();
                    }
                }
                else
                {
                    Movimento movimento = Movimento.Deserializar(str, new Movimento().GetType());

                    movimento.Deletar();
                }

                return Ok();
            }
            catch (SqlException e) // Exceção lançada pelo "Movimento".Deletar
            {
                return UnprocessableEntity(e.Message + Contact);
            }
            catch (JsonException e) // Exceção lançada pelo "Movimento".Deserializar
            {
                return BadRequest(e.Message + Contact);
            }
        }

    }
}
