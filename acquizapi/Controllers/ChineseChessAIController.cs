using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acquizapi.Models;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/ChineseChessAI")]
    public class ChineseChessAIController : Controller
    {
        // POST: api/ChineseChessAI
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/ChineseChessAI/5
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]ChineseChessAIInput value)
        {
            if (!ModelState.IsValid)
            {
#if DEBUG
                var vals = ModelState.Values;
                foreach (var val in vals)
                {                    
                    foreach (var err in val.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine(err.ErrorMessage);
                    }                    
                }
#endif

                return BadRequest("Failed to parse the inputting");
            }

            var output = new ChineseChessAIOutput();
            var state = new ChineseChessState();

            if (value.PlayingTeam == 1)
            {
                // Red
                // Oppo is black
                var agent = value.RedAgent;
                var oppoagent = value.BlackAgent;

                List<ChineseChessPiece> myPieces = new List<ChineseChessPiece>();
                foreach(var pc in agent.MyPieces)
                {
                    var cpc = new ChineseChessPiece(pc.Name, pc.Position[0], pc.Position[1]);
                    myPieces.Add(cpc);
                }
                ChineseChessAgent realagent = new ChineseChessAgent(1, myPieces);
                state.RedAgent = realagent;

                myPieces.Clear();
                foreach(var pc in oppoagent.MyPieces)
                {
                    var cpc = new ChineseChessPiece(pc.Name, pc.Position[0], pc.Position[1]);
                    myPieces.Add(cpc);
                }
                realagent = new ChineseChessAgent(-1, myPieces);
                state.BlackAgent = realagent;                
            }
            else if (value.PlayingTeam == -1)
            {
                // Black
                // Oppo is red
                var agent = value.BlackAgent;
                var oppoagent = value.RedAgent;

                List<ChineseChessPiece> myPieces = new List<ChineseChessPiece>();
                foreach (var pc in agent.MyPieces)
                {
                    var cpc = new ChineseChessPiece(pc.Name, pc.Position[0], pc.Position[1]);
                    myPieces.Add(cpc);
                }
                ChineseChessAgent realagent = new ChineseChessAgent(1, myPieces);
                state.BlackAgent = realagent;

                myPieces.Clear();
                foreach (var pc in oppoagent.MyPieces)
                {
                    var cpc = new ChineseChessPiece(pc.Name, pc.Position[0], pc.Position[1]);
                    myPieces.Add(cpc);
                }
                realagent = new ChineseChessAgent(-1, myPieces);
                state.RedAgent = realagent;
            }

            //if (dict.playingTeam == 1)
            //{
            //    var agentDict = dict.redAgent;
            //    var oppo = dict.blackAgent;
            //}
            //else
            //{
            //    var agentDict = dict.blackAgent;
            //    var oppo = dict.redAgent;
            //}
            //oppo = Agent.copyFromDict(oppo);
            //var agent;
            //// console.log(agentDict.strategy)
            //var is_repeating = this.check_repeating(agentDict);

            //if (agentDict.strategy == 0) agent = GreedyAgent.copyFromDict(agentDict);
            //if (agentDict.strategy == 1) agent = ABPruning.copyFromDict(agentDict);
            //if (agentDict.strategy == 2) agent = Reorder.copyFromDict(agentDict);
            //if (agentDict.strategy == 3) agent = TDLearner.copyFromDict(agentDict);
            //if (agentDict.strategy == 4) agent = TDLearnerTrained.copyFromDict(agentDict);
            //if (agentDict.strategy == 5) agent = MCTS.copyFromDict(agentDict);
            //var new_state;
            //if (dict.playingTeam == 1) new_state = new State(agent, oppo, dict.playingTeam);
            //else new_state = new State(oppo, agent, dict.playingTeam);
            //new_state.is_repeating = is_repeating;
            //return new_state;

            state.PlayingTeam = (Int16)value.PlayingTeam;            

            if (value.PlayingTeam == 1)
            {
                //state.RedAgent = value.RedAgent;
                //state.BlackAgent = value.BlackAgent;
            }
            else
            {
                
            }

            // Calculate the next move
            var bgnDate = DateTime.Now;

            var endDate = DateTime.Now;
            var spentTime = endDate - bgnDate;

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };

            return new JsonResult(output, setting);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
