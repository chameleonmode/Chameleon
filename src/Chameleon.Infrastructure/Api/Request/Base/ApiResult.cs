using System;
using System.IO;
using System.Text.Json;

namespace Chameleon.Infrastructure.Api
{
    internal class ApiResult<TResult>
    {
        private readonly Type ResultType = typeof(TResult);
        private bool IsResultArray => ResultType.IsArray;

        private string _json;
        public ApiResult(string json)
        {
            _json = json;
            ExtractData();
            FixJson();
        }

        private void ExtractData()
        {
            ExtractData("{\"result\":", ",\"targetUrl\":");
            ExtractData(",\"items\":", "}");
        }

        private void ExtractData(string prefix, string postfix)
        {
            var startIndex = _json.IndexOf(prefix);
            if (startIndex == -1)
            {
                return;
            }

            var endIndex = _json.LastIndexOf(postfix);
            if (endIndex == -1)
            {
                return;
            }

            startIndex += prefix.Length;
            _json = _json.Substring(startIndex, endIndex - startIndex);
        }

        public TResult Deserialize()
        {
            ThrowIfInvalidJson();

            return JsonSerializer.Deserialize<TResult>(_json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private void FixJson()
        {
            FixNullValue();
            FixWrappingQuotes();
            FixZeroId();
            FixObjectResponse();
            FixArrayResponse();
            FixEmptyArray();
        }

        private void FixNullValue()
        {
            if (!_json.Contains("\"null\""))
            {
                return;
            }

            LogError($"Expected null value instead of 'null' as string");
            _json = _json.Replace("\"null\"", "null");
        }

        private void FixObjectResponse()
        {
            if (IsResultArray)
            {
                return;
            }

            if (!_json.StartsWith("["))
            {
                return;
            }

            LogError($"Expected object instead of array");
            _json = _json.Trim('[', ']');
        }

        private void FixArrayResponse()
        {
            if (!IsResultArray)
            {
                return;
            }

            if (_json.StartsWith("["))
            {
                return;
            }

            LogError($"Expected array instead of single object");
            _json = $"[{_json}]";
        }

        private void FixEmptyArray()
        {
            if (!IsResultArray)
            {
                return;
            }

            const string emptyArrayBug = "[[]]";
            if (_json != emptyArrayBug)
            {
                return;
            }

            LogError($"Invalid array format");
            _json = _json.Replace(emptyArrayBug, "[]");
        }

        private void FixWrappingQuotes()
        {
            if (!_json.StartsWith("\""))
            {
                return;
            }

            LogError($"Unexpected surrounded quotes");
            _json = _json.Trim('\"');
        }

        private void FixZeroId()
        {
            const string invalidIdPropertyBug = "\"id\":0,";
            if (!_json.Contains(invalidIdPropertyBug))
            {
                return;
            }

            LogError($"Invalid property: {invalidIdPropertyBug}");
            _json = _json.Replace(invalidIdPropertyBug, string.Empty);
        }

        private void ThrowIfInvalidJson()
        {
            if (_json.Length == 0)
            {
                throw new InvalidDataException("Empty response");
            }

            var firstChar = _json[0];
            // check if it is a json string or not
            if (firstChar != '{' && firstChar != '[')
            {
                throw new InvalidDataException(_json);
            }
        }

        private void LogError(string message)
        {
            // TODO: add log error
        }
    }
}
