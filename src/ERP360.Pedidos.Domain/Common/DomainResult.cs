using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP360.Pedidos.Domain.Common
{
    public readonly struct DomainResult
    {
        public bool IsSuccess { get; }
        public string Error { get; }

        private DomainResult(bool success, string error)
        {
            IsSuccess = success;
            Error = error;
        }

        public static DomainResult Success()
        => new(true, string.Empty);

        public static DomainResult Failure(string error)
            => new(false, error ?? "Erro desconhecido.");
    }

}
