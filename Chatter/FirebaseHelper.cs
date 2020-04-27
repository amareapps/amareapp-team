using Firebase.Database;
using Firebase.Database.Query;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Chatter.Model;
using Org.BouncyCastle.Asn1;

namespace XamarinFirebase.Helper
{

    public class FirebaseHelper
    {
        /*
        FirebaseClient firebase = new FirebaseClient("https://chatter-7b8e4.firebaseio.com/");
        public async Task<List<UserModel>> GetAllUserModel()
        {

            return (await firebase
              .Child("UserModel")
              .OnceAsync<UserModel>()).Select(item => new UserModel
              {
             //     _id = item.Object._id,
              //    _username = item.Object._username
              }).ToList();
        }

        public async Task AddUserModel(int UserModelId, string name,string gender, string image,string email,string password)
        {

            //await firebase
            //  .Child("UserModel")
              //.PostAsync(new UserModel() {_id = UserModelId, _username = name,_gender = gender,_image = image,_password = password,_email = email });
        }

        public async Task<UserModel> GetUserModel(int UserModelId)
        {
          //  var allUserModel = await GetAllUserModel();
          //  await firebase
          //    .Child("UserModel")
           //   .OnceAsync<UserModel>();
         //   return allUserModel.Where(a => a._id == UserModelId).FirstOrDefault();
        }

        public async Task UpdateUserModel(int UserModelId, string name)
        {
           // var toUpdateUserModel = (await firebase
            //  .Child("UserModel")
             // .OnceAsync<UserModel>()).Where(a => a.Object._id == UserModelId).FirstOrDefault();

            //await firebase
             // .Child("UserModel")
             // .Child(toUpdateUserModel.Key)
             // .PutAsync(new UserModel() { _id = UserModelId, _username = name });
        }

        public async Task DeleteUserModel(int UserModelId)
        {
         //   var toDeleteUserModel = (await firebase
          //    .Child("UserModel")
          //    .OnceAsync<UserModel>()).Where(a => a.Object._id == UserModelId).FirstOrDefault();
            await firebase.Child("UserModel").Child(toDeleteUserModel.Key).DeleteAsync();

        }
        */
    }

}