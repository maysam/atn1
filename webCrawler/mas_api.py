import functools
import requests
import logging

from util import rate_limited

from config import MAS_APPID
from config import MAS_RATE_LIMIT

class MASError(Exception):
    """
    Base Exception thrown by the MAS Object when there is an error with the
    API.
    """

class MASResponse(object):
    """
    Response from a MAS Request
    """

class MAS(object):
    base_url = "http://academic.research.microsoft.com/json.svc/search"

    error_messages = (
        "The request succeeded",
        "The AppID has no access rights to the MAS API",
        "Invalid parameters given.",
        "The MAS service is temporarily unavailable",
        "The search condition is not supported yet",
    )

    result_objects = (
        "Author",
        "Publication",
        "Conference",
        "Journal",
        "Domain",
        "Organization",
        "Keyword",
    )

    def __getattr__(self, k):
        try:
            return object.__getattr__(self, k)
        except AttributeError:
            k = k.capitalize()
            assert k in self.result_objects

            def function(obj_type, **params):
                params['ResultObjects'] = obj_type
                return self.request(params)

            return functools.partial(function, k)

    def __init__(self):
        self.app_id = MAS_APPID

    @rate_limited(MAS_RATE_LIMIT[0],MAS_RATE_LIMIT[1])
    def request(self, params):
        params['AppId'] = self.app_id
        print "PARAMS", params
        result_objects = params['ResultObjects']
        resp = requests.request("GET", self.base_url, params=params)
        try:
            resp_data = resp.json['d']
        except:
            print "The json response has no 'd' key"
            print "Response TEXT", resp.raw
            print "Response JSON", resp.json
            raise

        # Check that the request is successful
        try:
            result_code = resp_data['ResultCode']
            assert result_code == 0

        except AssertionError:
            print self.error_messages[result_code]
        return resp_data[result_objects]

    @rate_limited(MAS_RATE_LIMIT[0],MAS_RATE_LIMIT[1])
    def get_publication_id(self, params):
        pass

    # MAS API will only return 100 results per query,
    # therefore it is necessary to have a rate-limited
    # function that breaks the process of getting citing
    # papers into smaller, still-rate-limited tasks.
    @rate_limited(MAS_RATE_LIMIT[0],MAS_RATE_LIMIT[1])
    def __get_next_citing_papers(self, publication_id, start_idx):
        pass

    @rate_limited(MAS_RATE_LIMIT[0],MAS_RATE_LIMIT[1])
    def get_citing_papers(self, publication_id):
        pass

    @rate_limited(MAS_RATE_LIMIT[0],MAS_RATE_LIMIT[1])
    def get_paper_data(self, publication_id):
        pass



class MASExample(object):
    def __init__(self):
        self.api = MAS()

    def author(self, author_id):
        resp = self.api.author({"AuthorID": author_id, "StartIdx": 1, "EndIdx": 1})
        try:
            return resp['Author']['Result'][0]
        except IndexError:
            print "No Author found with the specified AuthorID", author_id
            raise
        except: # KeyError should never happen
            print "Unknown Error"
            raise

