using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NBitcoin.Protocol;
using NBitcoin.Protocol.Behaviors;
using NBitcoin.SPV;
using NBitcoin;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Stock.Controllers
{
    [Route("api/[controller]")]
    abstract public class BaseController : Controller
    {
        internal BaseController()
        {
            StartConnecting();
        }

        internal async void StartConnecting()
        {
            await Task.Factory.StartNew(() =>
            {
                var parameters = new NodeConnectionParameters();
                parameters.TemplateBehaviors.Add(new AddressManagerBehavior(GetAddressManager())); //So we find nodes faster
                parameters.TemplateBehaviors.Add(new ChainBehavior(GetChain())); //So we don't have to load the chain each time we start
                parameters.TemplateBehaviors.Add(new TrackerBehavior(GetTracker())); //Tracker knows which scriptPubKey and outpoints to track, it monitors all your wallets at the same
                if (!_Disposed)
                {
                    _Group = new NodesGroup(Network.Main, parameters, new NodeRequirement()
                    {
                        RequiredServices = NodeServices.Network //Needed for SPV
                    });
                    _Group.MaximumNodeConnection = 4;
                    _Group.Connect();
                    _ConnectionParameters = parameters;
                }
            });
        }

        internal NodeConnectionParameters _ConnectionParameters;

        bool _Disposed;

        private ConcurrentChain GetChain()
        {
            if (_ConnectionParameters != null)
            {
                return _ConnectionParameters.TemplateBehaviors.Find<ChainBehavior>().Chain;
            }
            var chain = new ConcurrentChain(Network.Main);
            try
            {
                //lock (App.Saving)
                //{
                chain.Load(System.IO.File.ReadAllBytes(ChainFile()));
                //}
            }
            catch
            {
            }
            return chain;
        }

        private Tracker GetTracker()
        {
            if (_ConnectionParameters != null)
            {
                return _ConnectionParameters.TemplateBehaviors.Find<TrackerBehavior>().Tracker;
            }
            try
            {
                //     lock (App.Saving)
                //   {
                using (var fs = System.IO.File.OpenRead(TrackerFile()))
                {
                    return Tracker.Load(fs);
                }
                // }
            }
            catch
            {
            }
            return new Tracker();
        }

        private string TrackerFile()
        {
            return Path.Combine(AppContext.BaseDirectory, "tracker.dat");
        }

        private static string ChainFile()
        {
            return Path.Combine(AppContext.BaseDirectory, "chain.dat");
        }
        internal NodesGroup _Group;

        private AddressManager GetAddressManager()
        {
            if (_ConnectionParameters != null)
            {
                return _ConnectionParameters.TemplateBehaviors.Find<AddressManagerBehavior>().AddressManager;
            }
            try
            {
                //       lock (App.Saving)
                //     {
                return AddressManager.LoadPeerFile(AddrmanFile());
                //   }
            }
            catch
            {
                return new AddressManager();
            }
        }

        private static string AddrmanFile()
        {
            return Path.Combine(AppContext.BaseDirectory, "addrman.dat");
        }

    }
}
