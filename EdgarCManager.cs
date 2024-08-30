using System;
using System.Collections.Generic;
using System.Linq;
using DBu;
using log4net;
using log4net.Core;

namespace EdgarC
{
    public class EdgarCManager
    {

        private readonly IDownloader _downloader;
        private readonly ILog _log;
        private readonly ILog _ui;
        private readonly IDBuProvider _dbuProvider;

        public EdgarCManager(IDownloader downloader, ILog log, ILog ui, IDBuProvider dbuProvider)
        {
            _downloader = downloader;
            _log = log;
            _ui = ui;
            _dbuProvider = dbuProvider;
        }

        public void DownloadQuarterCurrent()
        {
            var month = DateTime.Now.Month;
            var quarter = month >= 1 && month <= 3 ? 1 : month >= 4 && month <= 6 ? 2 : month >= 7 && month <= 9 ? 3 : 4;
            this.DownloadQuarter(DateTime.Now.Year, quarter);
        }

        public void DownloadQuarter(int year, int quarter)
        {

            var results = _downloader.GetFormList(year, quarter).Result;

            var companies = new List<Company>();

            foreach (var r in results.Select(r => r.SECNumber).Distinct())
            {
                var result = results.FirstOrDefault(res => res.SECNumber == r);

                var c = new Company();
                _dbuProvider.Load(c, new Dictionary<string, object>() { { "SECNumber", r } });

                if (!c.Exists)
                {
                    c = result.GetCompany();

                    _dbuProvider.Save(c);
                    _ui.Info("+");
                }
                else
                {
                    _ui.Info(".");
                }

                companies.Add(c);
            }

            // save companies and forms?
            _ui.Info("Companies complete.");

            results.ForEach(r =>
            {
                _ui.Info($"result: {r}");
                // save each record.

                var c = companies.First(c1 => c1.SECNumber == r.SECNumber);

                var f = new Form()
                {
                    SECDocumentNumber = r.SECDocumentNumber,
                    CompanyId = c.Id,
                    FormType = r.FormType,
                    FullPath = r.FullPath
                };

                if (!f.Load(new Dictionary<string, object>() { { "SECDocumentNumber", r.SECDocumentNumber } }))
                {
                    f.Save();
                    _ui.Info("+");
                }
                else
                {
                    _ui.Info(".");
                }

            });

        }

        public List<Company> CompanySearch(string search)
        {
            var q = _dbuProvider.GetQueryable<Company>();

            var count = q.Count();
            Console.WriteLine($"Count: {count}");

            if (count > 0)
            {
                var a = q.Skip(10).First();
                Console.WriteLine($"First: {a.Name}");
            }

            return q.Where(c => c.Name.ToUpper().Contains(search.ToUpper())).ToList();
        }
    }
}
