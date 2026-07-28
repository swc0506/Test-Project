public enum Protocal : short
{
    //登录请求和响应
    LoginRequest = 1,
    LoginResponse = 2,
    //创建账号请求和响应
    CreateUserRequest = 3,
    CreateUserResponse = 4,
    //招募英雄请求和响应
    RecruitHeroRequest = 5,
    RecruitHeroResponse = 6,
    
    StartBattleRequest = 300,
    StartBattleResponse = 301,
    BattleResultRequest = 302,
    BattleResultResponse = 303,
    
}