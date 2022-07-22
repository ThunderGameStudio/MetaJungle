using Defective.JSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class CovalentManager : MonoBehaviour
{
    public static CovalentManager insta;

    string BalanceFetchPreURL = "https://api.covalenthq.com/v1/80001/address/";
    string BalanceFetchPostURL = "/balances_v2/?quote-currency=USD&format=JSON&nft=true&no-nft-fetch=true&key=ckey_8b741a7fb36a427ab1497d4c612";

    string GetMetaDataPreURL = "https://api.covalenthq.com/v1/80001/tokens/";
    string GetMetaDataMidURL = "/nft_metadata/";
    string GetMetaDataPostURL = "/?quote-currency=USD&format=JSON&key=ckey_8b741a7fb36a427ab1497d4c612";


    public List<string> myTokenID = new List<string>();

    public List<MyMetadataNFT> myNFTData = new List<MyMetadataNFT>();

    private void Awake()
    {
        insta = this;
    }


    public void GetNFTUserBalance()
    {
        StartCoroutine(GetNFTBalance());
    }
    IEnumerator GetNFTBalance()
    {
        yield return new WaitForSeconds(1f);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(BalanceFetchPreURL + SingletonDataManager.userethAdd + BalanceFetchPostURL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            //string[] pages = uri.Split('/');
            //int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: " + webRequest.downloadHandler.text);

                    JSONObject _data = new JSONObject(webRequest.downloadHandler.text);

                    if (_data.GetField("data").HasField("items"))
                    {
                        for (int i = 0; i < _data.GetField("data").GetField("items").list.Count; i++)
                        {
                            var _add = _data.GetField("data").GetField("items")[i].GetField("contract_address").stringValue.ToLower();
                            if (SingletonDataManager.insta.contract_ethAddress.ToLower().Equals(_add))
                            {
                                if (_data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count > 0)
                                {
                                    Debug.Log("Found :" + _add + " | NFT" + _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count);
                                    myNFTData.Clear();

                                    for (int j = 0; j < _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count; j++)
                                    {
                                        myTokenID.Add(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue);
                                        GetNFTMetaDataDetails(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue);
                                        yield return new WaitForSeconds(0.5f);
                                    }
                                }


                            }
                        }
                    }

                    break;
            }
        }
    }


    public void GetNFTMetaDataDetails(string _tid)
    {
        StartCoroutine(GetNFTMetaData(_tid));
    }
    IEnumerator GetNFTMetaData(string _tokenid)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GetMetaDataPreURL + SingletonDataManager.insta.contract_ethAddress + GetMetaDataMidURL + _tokenid + GetMetaDataPostURL))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            //string[] pages = uri.Split('/');
            //int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("Received: NFTMeta " + webRequest.downloadHandler.text);

                    JSONObject _data = new JSONObject(webRequest.downloadHandler.text);

                    if (_data.GetField("data").HasField("items"))
                    {
                        for (int i = 0; i < _data.GetField("data").GetField("items").list.Count; i++)
                        {
                            var _add = _data.GetField("data").GetField("items")[i].GetField("contract_address").stringValue.ToLower();
                            if (SingletonDataManager.insta.contract_ethAddress.ToLower().Equals(_add))
                            {
                                if (_data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count > 0)
                                {
                                    Debug.Log("Found :" + _add + " | NFT" + _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count);

                                    for (int j = 0; j < _data.GetField("data").GetField("items")[i].GetField("nft_data").list.Count; j++)
                                    {
                                        Debug.Log("Found Details token_id :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue);
                                        Debug.Log("Found Details item_id :" + (int.Parse(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue) - 200));
                                        Debug.Log("Found Details name :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("name").stringValue);
                                        Debug.Log("Found Details description :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("description").stringValue);
                                        Debug.Log("Found Details image :" + _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("image").stringValue);

                                        MyMetadataNFT _nftData = new MyMetadataNFT();
                                        _nftData.name = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("name").stringValue;
                                        _nftData.description = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("description").stringValue;
                                        _nftData.image = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("external_data").GetField("image").stringValue;
                                        _nftData.tokenId = _data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue;
                                        _nftData.itemid = (int.Parse(_data.GetField("data").GetField("items")[i].GetField("nft_data")[j].GetField("token_id").stringValue) - 200);
                                        //myNFTData.Add(_nftData);
                                        SingletonDataManager.myNFTData.Add(_nftData);


                                    }
                                }


                            }
                        }
                    }

                   
                        if (MetaManager.insta)
                        {
                            MetaManager.insta.UpdatePlayerWorldProperties();
                            Debug.Log("We UpdatePlayerWorldProperties");
                        }
                    
                    break;
            }
        }
    }
}
