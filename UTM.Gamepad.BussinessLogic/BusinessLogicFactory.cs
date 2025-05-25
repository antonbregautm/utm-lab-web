using UTM.Gamepad.BussinessLogic.BLogic;
using UTM.Gamepad.BussinessLogic.Interfaces;

namespace UTM.Gamepad.BussinessLogic
{
    public class BusinessLogicFactory
    {
        private static BusinessLogicFactory _instance;

        private BusinessLogicFactory() { }

        public static BusinessLogicFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessLogicFactory();
                }
                return _instance;
            }
        }

        public IAuthBL GetAuthBL()
        {
            return new AuthBL();
        }

        public IOrderBL GetOrderBL()
        {
            return new OrderBL();
        }
        
        public IUserBL GetUserBL()
        {
            return new UserBL();
        }
        
        public IRoleBL GetRoleBL()
        {
            return new RoleBL();
        }
        
        public IProductBL GetProductBL()
        {
            return new ProductBL();
        }
    }
} 