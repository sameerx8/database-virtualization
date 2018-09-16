using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Imposters;
using MbDotNet.Models.Predicates;
using MbDotNet.Models.Predicates.Fields;

namespace ServiceVirtualization.Database.SqlClient
{
    public class ServiceVirtMountebankPublisher
    {
        private static ConcurrentDictionary<string, List<VirtSqlRpcModel>> _recordedCommands = new ConcurrentDictionary<string, List<VirtSqlRpcModel>>();
        private static HttpClient _httpClient = new HttpClient();

        public static void AddRecordedCommand(VirtSqlRpcModel model)
        {
            _recordedCommands.AddOrUpdate(model.StoredProcName, s => new List<VirtSqlRpcModel> { model }, (s, list) =>
            {
                list.Add(model);
                return list;
            });

            //TODO: PUT to replace all imposters with all recordings each time a new one is added.
            //this is because the imposters list is immutable

            var mountebankClient = new MountebankClient();

            mountebankClient.DeleteImposter(1234);

            var imposter = mountebankClient.CreateHttpImposter(1234, "test");

            foreach (var virtRecordedCommand in _recordedCommands)
            {
                var stub = imposter.AddStub();

                var predicateFields = new HttpPredicateFields
                {
                    Path = virtRecordedCommand.Key,
                };

                var containsPredicate = new ContainsPredicate<HttpPredicateFields>(predicateFields);

                foreach (var response in virtRecordedCommand.Value)
                {
                    stub
                        .On(containsPredicate)
                        .ReturnsJson(HttpStatusCode.OK, response);
                }

            }

            mountebankClient.Submit(imposter);


        }
    }
}
