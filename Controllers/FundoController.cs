using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using FundoClass;

namespace Tentativa2APIviaJSON.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FundoController : ControllerBase
    {
        // GET: Fundo , retorna uma lista que contém todos os fundos 
        [HttpGet]
        public IActionResult Get()
        {
            List<Fundo> fundos = new Fundo().ListarTodos();
            if (fundos.Count < 1) 
            {
                return null;
            }
            
            return Ok(Fundo.Serializar(fundos));
        }

        // GET: Fundo/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Fundo fundo = new Fundo(id);
            if(string.IsNullOrEmpty(fundo.Nome)) 
            {
                return null;
            }

            return Ok(fundo.Serializar());
        }

        // POST: Fundo
        [HttpPost]
        public IActionResult Post([FromBody] JsonElement j)
        {
            string str = j.ToString();
            if (str.StartsWith("["))
            {
                List<Fundo> lista = Fundo.Deserializar(str);

                for(int i = 0; i < lista.Count; i++)
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

        // PUT: Fundo
        [HttpPut]
        public IActionResult Put([FromBody] JsonElement j)
        {
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

        // DELETE: Fundo
        [HttpDelete]
        public IActionResult Apagar([FromBody] JsonElement j)
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
    }
}
